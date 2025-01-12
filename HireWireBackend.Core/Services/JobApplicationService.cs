using HireWireBackend.Core.Interfaces.IServices;
using LibraryManegerBackend.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HireWireBackend.Core.Services;

public class JobApplicationService : IJobApplicationService
{

    private readonly IRepository _repository;

    public JobApplicationService(IRepository repository)
    {
        _repository = repository;
    }


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

    public async Task<int> GetResponsesCountForEmployer(int employerId)
    {
        var allApplications = await _repository.GetAll<JobApplication>().ToListAsync();
        var count = allApplications.Where(app => app.Vacancy.EmployerId == employerId && !app.IsDeleted.Value).Count();
        return count;
    }

    public async Task<IEnumerable<JobApplication>> GetResponsesForEmployer(int employerId, int page, int pageSize)
    {
        return await _repository.GetAll<JobApplication>()
            .Where(app => app.Vacancy.EmployerId == employerId && !app.IsDeleted.Value)
            .Include(app => app.Vacancy)
            .Include(app => app.Applicant)
            .ThenInclude(app => app.ApplicantNavigation)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}

