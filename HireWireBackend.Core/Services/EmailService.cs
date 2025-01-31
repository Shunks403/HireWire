using HireWireBackend.Core.Interfaces.IServices;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace HireWireBackend.Core.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string templatePath, Dictionary<string, string> replacements)
    {
        // Load the email template from the file system
        var templateContent = await File.ReadAllTextAsync(templatePath);

        // Replace placeholders in the template
        foreach (var replacement in replacements)
        {
            templateContent = templateContent.Replace($"{{{{{replacement.Key}}}}}", replacement.Value);
        }

        // Create the email message
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("HireWire", _configuration["EmailSettings:From"]));
        email.To.Add(new MailboxAddress("", toEmail));
        email.Subject = subject;

        // Set the email body (HTML content)
        email.Body = new TextPart("html")
        {
            Text = templateContent
        };

        // Send the email using SMTP
        using (var smtpClient = new SmtpClient())
        {
            try
            {
                await smtpClient.ConnectAsync(
                    _configuration["EmailSettings:SmtpServer"],
                    int.Parse(_configuration["EmailSettings:SmtpPort"]),
                    MailKit.Security.SecureSocketOptions.StartTls
                );
                await smtpClient.AuthenticateAsync(
                    _configuration["EmailSettings:From"],
                    _configuration["EmailSettings:Password"]
                );
                await smtpClient.SendAsync(email);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка отправки email: {ex.Message}");
            }
            finally
            {
                await smtpClient.DisconnectAsync(true);
            }
        }
    }
}