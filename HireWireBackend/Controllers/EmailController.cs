using HireWireBackend.Core.Interfaces.ILoggers;
using HireWireBackend.Core.Interfaces.IServices;
using HireWireBackend.DTO.Email;
using Microsoft.AspNetCore.Mvc;

namespace HireWireBackend.Controllers;

[ApiController]
[Route("api/email")]

public class EmailController: Controller
{
    readonly private IEmailService _emailService;
    private readonly IBlobLogger _logger;

    public EmailController(IEmailService emailService , IBlobLogger blobLogger)
    {
        _emailService = emailService;
        _logger = blobLogger;
    }
    
    [HttpPost("send-contact-notification")]
    public async Task<IActionResult> SendContactNotification([FromBody] ContactNotificationDto notificationDto)
    {
        try
        {
            if (string.IsNullOrEmpty(notificationDto.ContactMethod) || string.IsNullOrEmpty(notificationDto.ContactDetails))
            {
                return BadRequest("Contact method and details are required.");
            }

            var templateName = notificationDto.ContactMethod switch
            {
                "email" => "EmailNotificationTemplate.html",
                "phone" => "PhoneNotificationTemplate.html",
                "video_call" => "VideoCallNotificationTemplate.html",
                _ => throw new ArgumentException("Invalid contact method")
            };

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Emails", templateName);
        
            var replacements = new Dictionary<string, string>
            {
                { "UserName", notificationDto.ApplicantName },
                { "ContactDetails", notificationDto.ContactDetails },
                { "VideoService", notificationDto.VideoService ?? "" },
                {"Email",notificationDto.ContactDetails}
            };

            await _emailService.SendEmailAsync(notificationDto.ApplicantEmail, "Meeting Notification", templatePath, replacements);
        
            return Ok("Notification sent successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error sending notification: {ex.Message}");
        }
    }

}