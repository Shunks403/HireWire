namespace HireWireBackend.DTO;

public class JobVacancyWithTagsDTO
{
    public JobVacancy JobVacancy { get; set; }
    public List<string> Tags { get; set; }
}