namespace HireWireBackend.DTO;

public class CreateVacancyWithTagsDTO
{
    public JobVacancyDTO JobVacancy { get; set; }
    public List<string> Tags { get; set; }
}