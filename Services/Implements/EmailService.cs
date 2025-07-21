using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Netflex.Models.Configs;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Netflex.Services.Implements;
public class EmailService : IEmailSender
{
    private readonly EmailConfig _emailConfig;

    public EmailService(IOptions<EmailConfig> emailConfig)
    {
        _emailConfig = emailConfig.Value;
    }

   public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
{
    var apiKey = _emailConfig.Key;
    var client = new SendGridClient(apiKey);
    var from = new EmailAddress(_emailConfig.OwnerMail, _emailConfig.Company);
    var to = new EmailAddress(toEmail);

    var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent); // HTML content
    var response = await client.SendEmailAsync(msg);

    // (Optional) Log response status
    if (!response.IsSuccessStatusCode)
    {
        var body = await response.Body.ReadAsStringAsync();
        Console.WriteLine($"SendGrid Error: {body}");
    }
}

}