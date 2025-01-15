namespace HireWireBackend.DTO;

public class JobVacancyCompactDTO
{
    public int VacancyId { get; set; }
    public string Title { get; set; }
    
    public string Status { get; set; }
    
    public string CompanyName { get; set; }
    
    public string Requirements { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public decimal Salary { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Tags { get; set; }
}