namespace HireWireBackend.Core.Interfaces.IServices;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string templatePath, Dictionary<string, string> replacements);
}