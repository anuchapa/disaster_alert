using System;
using SendGrid;

namespace DisasterAlarm.service.Services.MessagingService;

public interface IMessagingService
{
    Task<Response> SendEmail(string reciever,string subject,string plainTextContent,string htmlContent);
}
