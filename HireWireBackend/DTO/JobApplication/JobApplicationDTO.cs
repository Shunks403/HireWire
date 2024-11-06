namespace HireWireBackend.DTO;

public class JobApplicationDTO
{
    public int ApplicationId { get; set; }
    
    public int? VacancyId { get; set; }

    public int? ApplicantId { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}