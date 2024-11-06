namespace HireWireBackend.DTO;

public class ApplicantDTO
{
    public int ApplicantId { get; set; }

    public string? Resume { get; set; }

    public string? Skills { get; set; }

    public string? Education { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}