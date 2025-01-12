namespace HireWireBackend.DTO;

public class ApplicantDTO
{
    public int UserId { get; set; }
    public string? Skills { get; set; }
    public string? Education { get; set; }
    
    public IFormFile File { get; set; } 
}