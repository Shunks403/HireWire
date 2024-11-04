using HireWireBackend.Core.Interfaces.IServices;
using LibraryManegerBackend.Core.Interfaces;

namespace HireWireBackend.Core.Services;

public class JobVacancyService : IJobVacancyService
{
    private readonly IRepository _repository;

    public JobVacancyService(IRepository repository)
    {
        _repository = repository;
    }


    public Task<JobVacancy> Update(JobVacancy entity)
    {
        return _repository.Update(entity);
    }

    public Task<JobVacancy> FindById(int id)
    {
        return _repository.GetById<JobVacancy>(id);
    }

    public Task Delete(int id)
    {
        var vacancy = FindById(id).Result;
        vacancy.IsDeleted = true;
        Update(vacancy);
        
        return Task.CompletedTask;
    }

    public Task<JobVacancy> Add(JobVacancy entity)
    {
        return _repository.Add(entity);
    }

    public IEnumerable<JobVacancy> GetJobVacanciesEmployer(int EmployerId)
    {
        var listJobVacancies = _repository.GetAll<JobVacancy>().Where(x => x.EmployerId == EmployerId).ToList();
        return listJobVacancies;
    }

    public IEnumerable<JobVacancy> GetAll()
    {
        throw new NotImplementedException();
    }
}