using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace DisasterAlert.service.Services.MessagingService;

public class TwilioService : IMessagingService
{
    private readonly TwilioServiceSetting _setting;
    private readonly SendGridClient _client;
    private readonly ILogger<TwilioService> _logger;
    public TwilioService(IOptions<TwilioServiceSetting> setting, ILogger<TwilioService> logger)
    {
        _setting = setting.Value;
        _client = new SendGridClient(_setting.SengridApiKey);
        _logger = logger;
    }

    public async Task<Response> SendEmail(string reciever, string subject, string plainTextContent, string htmlContent)
    {
        var from = new EmailAddress(_setting.SenderEmail);
        var to = new EmailAddress(reciever);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var response = await _client.SendEmailAsync(msg);
        if (response.IsSuccessStatusCode)
            _logger.LogInformation("Email sent to {To} succesfully\n message:{PlainText} \nhtml: {HtmlText}", to, plainTextContent, htmlContent);
        else
            _logger.LogInformation("Email sent to {To} unsuccesfully\n message:{PlainText} \nhtml: {HtmlText}", to, plainTextContent, htmlContent);
        return response;
    }
}
