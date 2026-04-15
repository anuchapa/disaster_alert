using System;

namespace DisasterAlert.service.Services.MessagingService;

public class TwilioServiceSetting
{
    public string SengridApiKey {get; set;} = string.Empty;
    public string SenderEmail {get; set;} = string.Empty;
}
