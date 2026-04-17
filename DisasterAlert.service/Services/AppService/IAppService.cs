using System;
using DisasterAlert.service.Dtos.Request;
using DisasterAlert.service.Dtos.Response;
using SendGrid;

namespace DisasterAlert.service.Services;

public interface IAppService
{
    Task<ServiceResponse<bool>> CreateRegionAsync(CreateRegionRequest[] request);
    Task<ServiceResponse<bool>> AlertSettingsAsync(AlertSettingRequest[] request);
    Task<ServiceResponse<RiskReportResponse[]>> CreateRiskReportAsync();
    Task<ServiceResponse<bool>> SendAlert();
    Task<ServiceResponse<RecentAlertResponse[]>> GetRecentAlerts();
}
