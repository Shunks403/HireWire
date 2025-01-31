using HireWireBackend.Core.Interfaces.IServices;
using HireWireBackend.Core.Models;
using LibraryManegerBackend.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HireWireBackend.Core.Services;

public class JobVacancyTagService : IJobVacancyTagService
{
    private readonly IRepository _repository;

    public JobVacancyTagService(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<JobVacancyTag> Add(JobVacancyTag jobVacancyTag)
    {
        return await _repository.Add(jobVacancyTag);
    }

    public async Task Delete(int id)
    {
        await _repository.Delete<JobVacancyTag>(id);
    }
    
    public async Task DeleteVacancyTagAsync(int vacancyId, int tagId)
    {
        var vacancyTag = await _repository.FirstOrDefaultAsync<JobVacancyTag>(jt => jt.VacancyId == vacancyId && jt.TagId == tagId);

        if (vacancyTag != null)
        {
            await _repository.Delete<JobVacancyTag>(vacancyTag.Id);
        }
    }
    
    
    
    public async Task<JobVacancyTag> Get(int id)
    {
        return await _repository.GetById<JobVacancyTag>(id);
    }

    public IQueryable<JobVacancyTag> GetAll()
    {
        return _repository.GetAll<JobVacancyTag>();
    }
    
    public async Task<List<Tag>> GetTagsByVacancyIdAsync(int vacancyId)
    {
        return await _repository.GetAll<JobVacancyTag>()
            .Where(jvt => jvt.VacancyId == vacancyId)
            .Select(jvt => jvt.Tag)
            .ToListAsync();
    }
}