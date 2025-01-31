namespace HireWireBackend.Core.Interfaces.IServices;

public interface IJobVacancyService : IBaseService<JobVacancy>
{
    Task<JobVacancy> Add(JobVacancy jobVacancy);

    IEnumerable<JobVacancy> GetJobVacanciesEmployer(int EmployerId);
    
    Task<(IEnumerable<JobVacancy> Jobs, int TotalPages)> GlobalSearch(string keywords, string location, int page, int pageSize);
}