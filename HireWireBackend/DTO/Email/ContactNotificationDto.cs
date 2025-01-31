namespace HireWireBackend.DTO.Email;

public class ContactNotificationDto
{
    public string ContactMethod { get; set; }
    public string ContactDetails { get; set; }
    
    public string ApplicantName { get; set; }
    
    public string ApplicantEmail { get; set; }
    public string? VideoService { get; set; }
    
   
}