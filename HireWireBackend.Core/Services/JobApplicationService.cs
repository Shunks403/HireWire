using HireWireBackend.Core.Interfaces.IServices;
using LibraryManegerBackend.Core.Interfaces;

namespace HireWireBackend.Core.Services;

public class JobApplicationService : IJobApplicationService
{

    private readonly IRepository _repository;
    
    
    public Task<JobApplication> Update(JobApplication entity)
    {
        return _repository.Update(entity);
    }

    public Task<JobApplication> FindById(int id)
    {
        return _repository.GetById<JobApplication>(id);
    }

    public Task Delete(int id)
    {
        var jobApplication = FindById(id).Result;
        jobApplication.IsDeleted = true;
        jobApplication.DeletedAt = DateTime.Today;
        return Task.CompletedTask;
    }

    public IEnumerable<JobApplication> GetAll()
    {
        return _repository.GetAll<JobApplication>();
    }

    public Task<JobApplication> Add(JobApplication jobApplication)
    {
        return _repository.Add(jobApplication);
    }
}

