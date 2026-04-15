using System;
using DisasterAlarm.service.Dtos.Request;
using DisasterAlarm.service.Dtos.Response;
using SendGrid;

namespace DisasterAlarm.service.Services;

public interface AppService
{
    Task<ServiceResponse<bool>> CreateRegionAsync(CreateRegionRequest[] request);
    Task<ServiceResponse<bool>> AlertSettingsAsync(AlertSettingRequest[] request);
    Task<ServiceResponse<RiskReportResponse[]>> CreateRiskReportAsync();
    Task<ServiceResponse<bool>> SendEmail();
    Task DisasterRisksAsync();
}
