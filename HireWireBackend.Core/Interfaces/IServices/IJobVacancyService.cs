namespace HireWireBackend.Core.Interfaces.IServices;

public interface IJobVacancyService : IBaseService<JobVacancy>
{
    Task<JobVacancy> Add(JobVacancy jobVacancy);

    IEnumerable<JobVacancy> GetJobVacanciesEmployer(int EmployerId);
}