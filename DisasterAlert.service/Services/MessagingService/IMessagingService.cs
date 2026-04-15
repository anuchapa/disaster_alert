using System;
using SendGrid;

namespace DisasterAlert.service.Services.MessagingService;

public interface IMessagingService
{
    Task<Response> SendEmail(string reciever,string subject,string plainTextContent,string htmlContent);
}
