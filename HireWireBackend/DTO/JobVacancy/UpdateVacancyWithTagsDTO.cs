namespace HireWireBackend.DTO;

public class UpdateVacancyWithTagsDTO
{
    public JobVacancyDTO JobVacancy { get; set; }
    public List<string> Tags { get; set; }
}