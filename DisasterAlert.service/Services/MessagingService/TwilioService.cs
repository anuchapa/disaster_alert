using System;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace DisasterAlert.service.Services.MessagingService;

public class TwilioService : IMessagingService
{
    private readonly TwilioServiceSetting _setting;
    private readonly SendGridClient _client;
    public TwilioService(IOptions<TwilioServiceSetting> setting)
    {
        _setting = setting.Value;
        _client = new SendGridClient(_setting.SengridApiKey);
    }

    public async Task<Response> SendEmail(string reciever,string subject,string plainTextContent,string htmlContent)
    {
        var from = new EmailAddress(_setting.SenderEmail);
        var to = new EmailAddress(reciever);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var response = await _client.SendEmailAsync(msg);
        return response;
    }
}
