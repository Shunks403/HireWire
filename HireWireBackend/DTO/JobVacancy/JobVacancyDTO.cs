namespace HireWireBackend.DTO;

public class JobVacancyDTO
{ 
        public int VacancyId { get; set; }
    
        public int? EmployerId { get; set; }
    
        public string? Title { get; set; }
    
        public string? Description { get; set; }
    
        public string? Requirements { get; set; }
    
        public decimal? Salary { get; set; }
    
        public string? Location { get; set; }
    
        public string? Status { get; set; }
    
        public DateTime? CreatedAt { get; set; }
    
        public DateTime? UpdatedAt { get; set; }
    
}