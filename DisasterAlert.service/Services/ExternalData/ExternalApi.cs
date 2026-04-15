using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;
using System.Web;
using DisasterAlert.service.Dtos.Response;
using Microsoft.Extensions.Options;

namespace DisasterAlert.service.Services;

public class ExternalApi : ExternalData
{
    private readonly HttpClient _httpClient;
    private readonly ExternalApiSetting _setting;
    public ExternalApi(HttpClient httpClient , IOptions<ExternalApiSetting> setting)
    {
        _httpClient = httpClient;
        _setting = setting.Value;
    }

    public async Task<OpenWeatherResponse?> GetCurrentWeatherAsync(double lat, double lon)
    {
        UriBuilder uriBuilder = new UriBuilder(_setting.OpenWeatherUrl);
        var paramValues = HttpUtility.ParseQueryString(uriBuilder.Query);
        paramValues.Add("lat", lat.ToString());
        paramValues.Add("lon", lon.ToString());
        paramValues.Add("exclude", "hourly,daily,alerts,minutely");
        paramValues.Add("units", "metric");
        paramValues.Add("appid", _setting.OpenWeatherApiKey);
        uriBuilder.Query = paramValues.ToString();
        string url = uriBuilder.ToString();
        HttpResponseMessage resp = await _httpClient.GetAsync(url);
        if (resp.IsSuccessStatusCode)
        {
            var data = await resp.Content.ReadFromJsonAsync<OpenWeatherResponse>();
            return data;
        }
        else
        {
            return null;
        }
    }

    public async Task<EarthquakeResponse?> GetEarthquakeDataAsync(double lat, double lon)
    {
        UriBuilder uriBuilder = new UriBuilder(_setting.EarthquakeUrl);
        var paramValues = HttpUtility.ParseQueryString(uriBuilder.Query);
        paramValues.Add("latitude", lat.ToString());
        paramValues.Add("longitude", lon.ToString());
        paramValues.Add("format", "geojson");
        paramValues.Add("maxradiuskm", "200");
        var now = DateTime.UtcNow.ToString("yyyy-MM-dd");
        paramValues.Add("starttime", now );
        uriBuilder.Query = paramValues.ToString();
        string url = uriBuilder.ToString();
        HttpResponseMessage resp = await _httpClient.GetAsync(url);
        if (resp.IsSuccessStatusCode)
        {
            var data = await resp.Content.ReadFromJsonAsync<EarthquakeResponse>();
            return data;
        }
        else
        {
            return null;
        }
    }
}
