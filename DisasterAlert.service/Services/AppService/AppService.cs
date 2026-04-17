using System;
using System.Collections.Concurrent;
using System.Transactions;
using DisasterAlert.context;
using DisasterAlert.context.Entities;
using DisasterAlert.service.Dtos.model;
using DisasterAlert.service.Dtos.Request;
using DisasterAlert.service.Dtos.Response;
using DisasterAlert.service.Services.CacheService;
using DisasterAlert.service.Services.MessagingService;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using SendGrid;
using SendGrid.Helpers.Mail.Model;

namespace DisasterAlert.service.Services;

public class AppService : IAppService
{
    private readonly DisasterAlertContext _context;
    private readonly IExternalData _external;
    private readonly ICacheService _cache;
    private readonly IMessagingService _messenger;
    public AppService(DisasterAlertContext context, IExternalData external, ICacheService cache, IMessagingService messenger)
    {
        _context = context;
        _external = external;
        _cache = cache;
        _messenger = messenger;
    }

    public async Task<ServiceResponse<bool>> CreateRegionAsync(CreateRegionRequest[] request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var inputdisasterType = request.SelectMany(r => r.DisasterTypes.Select(s => s.ToLower())).Distinct().ToArray();
            var existdisasterType = await _context.DisasterTypes.Where(d => inputdisasterType.Contains(d.Disaster)).ToArrayAsync();

            var latLonPairs = request.Select(r => r.LocationCoordinates.Latitude + "_" + r.LocationCoordinates.Longitude).ToArray();
            var existRegions = await _context.Regions.Where(r => latLonPairs.Contains(r.Latitude + "_" + r.Longitude)).ToArrayAsync();
            //var existRegions = await _context.Regions.Where(r => latLonPairs.Contains(new(r.Latitude, r.Longitude))).ToArrayAsync();

            var existdisasterTypeMap = existdisasterType.ToDictionary(d => d.Disaster);
            var existRegionsMap = existRegions.ToDictionary(r => (r.Latitude, r.Longitude));

            var regionDisasters = new List<RegionDisaster>();
            var regionDisasterPairs = new List<string>();

            foreach (var item in request)
            {
                var latLonPair = (item.LocationCoordinates.Latitude, item.LocationCoordinates.Longitude);
                if (!existRegionsMap.TryGetValue(latLonPair, out var region))
                {
                    region = new Region { RegionId = item.RegionId, Latitude = item.LocationCoordinates.Latitude, Longitude = item.LocationCoordinates.Longitude };
                    _context.Regions.Add(region);
                    existRegionsMap[latLonPair] = region;
                }

                foreach (var type in item.DisasterTypes)
                {
                    var typeLower = type.ToLower();
                    if (!existdisasterTypeMap.TryGetValue(typeLower, out var disType))
                    {
                        disType = new DisasterType { Disaster = typeLower };
                        _context.DisasterTypes.Add(disType);
                        existdisasterTypeMap[typeLower] = disType;
                    }
                    else
                    {
                        if (region.Id != 0 && disType.Id != 0)
                            regionDisasterPairs.Add(region.Id + "_" + disType.Id);
                    }

                    var regionDisaster = new RegionDisaster
                    {
                        RegionId = region.Id,
                        DisasterTypeId = disType.Id,
                        Region = region,
                        DisasterType = disType,
                    };
                    regionDisasters.Add(regionDisaster);
                }

            }

            if (regionDisasterPairs.Count() > 0)
            {
                var existRegionDisasterPairs = await _context.RegionDisasters.Where(r => regionDisasterPairs.
                Contains(r.RegionId + "_" + r.DisasterTypeId)).Select(r => r.RegionId + "_" + r.DisasterTypeId).AsNoTracking().ToListAsync();
                regionDisasters = regionDisasters.ExceptBy(existRegionDisasterPairs, r => r.RegionId + "_" + r.DisasterTypeId).ToList();
            }


            if (regionDisasters.Count() > 0)
                _context.RegionDisasters.AddRange(regionDisasters);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return ServiceResponse<bool>.Ok(true, "Create Sucessfully");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ServiceResponse<bool>.Fail(ex.Message);
        }
    }
    public async Task<ServiceResponse<bool>> AlertSettingsAsync(AlertSettingRequest[] request)
    {
        try
        {
            var inputRegion = request.Select(d => d.RegionId).ToArray();
            var inputdisasterType = request.Select(d => d.DisasterType).ToArray();
            var disasterTypeMap = await _context.DisasterTypes.Where(d => inputdisasterType.Contains(d.Disaster)).ToDictionaryAsync(r => r.Disaster);
            var regionMap = await _context.Regions.Where(r => inputRegion.Contains(r.RegionId)).ToDictionaryAsync(r => r.RegionId);
            var regionDisasterPairs = request.Select(d =>
            {
                if (disasterTypeMap.TryGetValue(d.DisasterType, out var disasterType) && regionMap.TryGetValue(d.RegionId, out var region))
                {
                    return region.Id + "_" + disasterType.Id;
                }
                return null;
            }

             ).ToArray();
            var regionDisasterMap = await _context.RegionDisasters.Where(r => regionDisasterPairs
            .Contains(r.RegionId + "_" + r.DisasterTypeId)).ToDictionaryAsync(r => (r.RegionId, r.DisasterTypeId));

            foreach (var item in request)
            {
                var pair = (regionMap[item.RegionId].Id, disasterTypeMap[item.DisasterType].Id);
                if (regionDisasterMap.TryGetValue(pair, out var regionDisaster))
                {
                    regionDisaster.Threshold = item.ThresholdScore;
                }
            }

            _context.RegionDisasters.UpdateRange(regionDisasterMap.Values);
            await _context.SaveChangesAsync();
            await _cache.DeletetAsync("riskReport");
            return ServiceResponse<bool>.Ok(true, "Update Sucessfully");
        }
        catch (Exception ex)
        {
            return ServiceResponse<bool>.Fail(ex.ToString());
        }
    }
    public async Task<ServiceResponse<RiskReportResponse[]>> CreateRiskReportAsync()
    {
        var riskReportsCache = await _cache.GetAsync<RiskReportResponse[]>("riskReport");
        if (riskReportsCache != null)
        {
            Console.WriteLine("Redis hit.");
            return ServiceResponse<RiskReportResponse[]>.Ok(riskReportsCache);
        }
        Console.WriteLine("Redis miss.");

        var regions = await _context.Regions.ToArrayAsync();
        var weatherResult = new ConcurrentBag<WeatherRegionModel>();

        await Parallel.ForEachAsync(regions, async (region, token) =>
        {
            var task1 = _external.GetCurrentWeatherAsync(region.Latitude, region.Longitude);
            var task2 = _external.GetEarthquakeDataAsync(region.Latitude, region.Longitude);
            await Task.WhenAll(task1, task2);
            var weatherData = await task1;
            var earthQuakeData = await task2;
            weatherResult.Add(new WeatherRegionModel
            {
                RegionId = region.RegionId,
                WeatherRespones = weatherData,
                EarthquakeRespones = earthQuakeData
            });
        });

        List<string> errorMessages = [];
        var weatherResultArray = weatherResult.ToArray();
        foreach (var item in weatherResultArray)
        {
            if (!item.EarthquakeRespones.Success)
            {
                errorMessages.Add($"RegionId {item.RegionId} : {item.ErrorMessage} ");
            }

            if (!item.WeatherRespones.Success)
            {
                errorMessages.Add($"RegionId {item.RegionId} : {item.ErrorMessage} ");
            }
        }

        if (errorMessages.Any())
        {
            var error = ServiceResponse<RiskReportResponse[]>.Fail("External api has errors.");
            error.Errors.AddRange(errorMessages);
            return error;
        }


        var weatherResultDict = weatherResultArray.ToDictionary(r => r.RegionId);

        var riskReports = await _context.RegionDisasters.Where(r => r.Threshold != null).Select(r =>
             new RiskReportResponse
             {
                 RegionId = r.Region.RegionId,
                 DisasterType = r.DisasterType.Disaster,
                 RiskScore = r.Threshold.Value
             }
        ).AsNoTracking().ToArrayAsync();

        foreach (var item in riskReports)
        {
            if (weatherResultDict.TryGetValue(item.RegionId, out var weather))
            {
                var threshold = item.RiskScore;
                var riskPercent = 0.0;
                switch (item.DisasterType)
                {
                    case "flood":
                        var rain = weather.WeatherRespones != null && weather.WeatherRespones.Current.Rain != null ? weather.WeatherRespones.Current.Rain.H1 : 0;
                        item.RiskScore = (float)rain;
                        break;
                    case "wildfire":
                        var humidity = weather.WeatherRespones != null ? weather.WeatherRespones.Current.Humidity : 0;
                        var temp = weather.WeatherRespones != null ? weather.WeatherRespones.Current.Temp : 0;
                        item.RiskScore = (float)(humidity > 0 ? temp / humidity / 1.2 * 100 : 0);
                        break;
                    case "earthquake":
                        var earthquake = weather.EarthquakeRespones != null ? weather.EarthquakeRespones.Features
                        .Aggregate(0.0, (sum, s) => sum + s.Properties.Mag) : 0.0;
                        earthquake = weather.EarthquakeRespones != null ? earthquake / weather.EarthquakeRespones.Features.Count() : 0;
                        item.RiskScore = (float)earthquake;
                        break;
                    default: break;
                }
                riskPercent = item.RiskScore / threshold * 100;
                item.RiskLevel = riskPercent < 50 ? "Low" : riskPercent < 80 ? "Medium" : "High";
                if (item.RiskLevel == "High")
                    item.AlertTrigger = true;
            }
        }
        _ = _cache.SetAsync("riskReport", riskReports);
        return ServiceResponse<RiskReportResponse[]>.Ok(riskReports);

    }

    public async Task<ServiceResponse<bool>> SendAlert()
    {
        var riskReportsCache = await _cache.GetAsync<List<RiskReportResponse>>("riskReport");
        if (riskReportsCache == null)
        {
            return ServiceResponse<bool>.Fail("There is no alert required for submission.");
        }
        var riskReportsCacheCount = riskReportsCache.Count();
        var riskReportsCacheDict = riskReportsCache.Where(r => r.AlertTrigger).GroupBy(r => r.RegionId).ToDictionary(r => r.Key, r => r.ToArray());
        if (!riskReportsCacheDict.Any())
        {
            return ServiceResponse<bool>.Fail("There is no alert required for submission.");
        }
        var regionIds = riskReportsCacheDict.Keys.ToArray();
        // // if (riskReportsCache == null)
        // // {
        // //    return 
        // // }


        var regionMap = _context.Regions.Where(r => regionIds.Contains(r.RegionId)).ToDictionary(r => r.Id);
        var users = _context.Users.Include(u => u.userSubscriptions).ToArray();
        if(users == null || !users.Any())
            return ServiceResponse<bool>.Fail("Empty reciever.");
        string subject = "Disaster alert";
        List<RiskAlert> riskAlerts = [];
        foreach (var user in users)
        {
            string reciever = user.Email;
            foreach (var sub in user.userSubscriptions)
            {
                if (regionMap.TryGetValue(sub.RegionId, out var region))
                {
                    if (riskReportsCacheDict.TryGetValue(region.RegionId, out var risks))
                    {
                        foreach (var risk in risks)
                        {
                            var alertContent = CreateAlertTemplate(risk);
                            var resp = await _messenger.SendEmail(reciever, subject, alertContent.Text, alertContent.Html);
                            riskAlerts.Add(alertContent.Alert);
                            if (resp.IsSuccessStatusCode)
                            {
                                alertContent.Alert.SuccessStatus = true;
                                riskReportsCache.Remove(risk);
                            }
                        }
                    }
                }

            }
            _context.AddRange(riskAlerts);

        }
        if (riskReportsCacheCount != riskReportsCache.Count())
        {
            _ = _cache.SetAsync("riskReport", riskReportsCache);
        }
        await _context.SaveChangesAsync();
        // string reciever = "reciever";
        // string subject = "Disaster alert";
        // string plainTextContent = "alert text test from sendgrid";

        // string htmlTemplate = File.ReadAllText("DisasterAlert.service/Template/Html/AlertTempalte.html");
        // string htmlContent = htmlTemplate
        //     .Replace("{{RegionId}}", "TH-001")
        //     .Replace("{{DisasterType}}", "Flood")
        //     .Replace("{{RiskLevel}}", "High")
        //     .Replace("{{AlertMessage}}", "Water level is rising rapidly.")
        //     .Replace("{{Timestamp}}", DateTime.Now.ToString("f"));

        // var resp = await _messenger.SendEmail(reciever, subject, plainTextContent, htmlContent);
        return ServiceResponse<bool>.Ok(true);
    }

    private AlertContentModel CreateAlertTemplate(RiskReportResponse risk)
    {
        var alertMessage = "";
        switch (risk.DisasterType)
        {
            case "flood":
                alertMessage = "Beware of flooding in this area.";
                break;
            case "wildfire":
                alertMessage = "Beware of wildfire in this area.";
                break;
            case "earthquake":
                alertMessage = "Be aware of the potential impact of earthquakes in this area.";
                break;
            default:
                break;
        }
        string htmlTemplate = File.ReadAllText("Template/Html/AlertTempalte.html");
        var now = DateTime.UtcNow;
        string htmlContent = htmlTemplate
            .Replace("{{regionId}}", risk.RegionId)
            .Replace("{{disasterType}}", risk.DisasterType)
            .Replace("{{riskLevel}}", risk.RiskLevel)
            .Replace("{{alertMessage}}", alertMessage)
            .Replace("{{timestamp}}", now.ToString("yyyy-MM-dd HH:mm:ss"));
        var alert = new RiskAlert
        {
            RegionId = risk.RegionId,
            DisasterType = risk.DisasterType,
            AlertMessage = alertMessage,
            RiskLevel = risk.RiskLevel,
            Timestamp = now
        };
        var alertContent = new AlertContentModel { Alert = alert, Text = alertMessage, Html = htmlContent };
        return alertContent;
    }
    public async Task<ServiceResponse<RecentAlertResponse[]>> GetRecentAlerts()
    {
        var r = await _context.RiskAlerts.GroupBy(r => r.RegionId).Select(r => r.FirstOrDefault()).ToArrayAsync();
        var recentAlert = await _context.RiskAlerts.GroupBy(r => r.RegionId).Select(
            r => r.OrderByDescending(r => r.Timestamp)
            .Select(r => new RecentAlertResponse
            {
                RegionId = r.RegionId,
                DisasterType = r.DisasterType,
                AlertMessage = r.AlertMessage,
                Timestamp = r.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                RiskLevel = r.RiskLevel,
                SuccessStatus = r.SuccessStatus
            }).FirstOrDefault()).ToArrayAsync();

        if (recentAlert == null && !recentAlert.Any())
        {
            return ServiceResponse<RecentAlertResponse[]>.Fail("์No alert found.");
        }

        return ServiceResponse<RecentAlertResponse[]>.Ok(recentAlert);

    }
}
