using HireWireBackend.Core.Models;

namespace HireWireBackend.Core.Interfaces.IServices;

public interface IJobVacancyTagService
{
    Task<JobVacancyTag> Add(JobVacancyTag jobVacancyTag);
    Task Delete(int id);
    Task<JobVacancyTag> Get(int id);
    IQueryable<JobVacancyTag> GetAll();
    
    Task<List<Tag>> GetTagsByVacancyIdAsync(int vacancyId);
}