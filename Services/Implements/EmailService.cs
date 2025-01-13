namespace Netflex.Services.Implements;
public class EmailService : IEmailService
{
    public Task<bool> SendEmailAsync(string email, string subject, string message)
    {
        throw new NotImplementedException();
    }
}