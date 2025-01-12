using HireWireBackend.Core.Interfaces.IServices;
using LibraryManegerBackend.Core.Interfaces;

namespace HireWireBackend.Core.Services;

public class ApplicantService : IApplicantService
{
    private readonly IRepository _repository;

    public ApplicantService(IRepository repository)
    {
        _repository = repository;
    }

    public Task<Applicant> Update(Applicant entity)
    {
        return _repository.Update(entity);
    }

    public Task<Applicant> FindById(int id)
    {
        return _repository.GetById<Applicant>(id);
    }

    public Task Delete(int id)
    {
        var applicant = FindById(id).Result;
        applicant.IsDeleted = true;
        applicant.DeletedAt = DateTime.Today;
        return Task.CompletedTask;
    }

    public IEnumerable<Applicant> GetAll()
    {
        return _repository.GetAll<Applicant>();
    }

    public Task<Applicant> Add(Applicant applicant)
    {
       return _repository.Add(applicant);
    }
    
    
    public async Task<Applicant> CreateApplicant(Applicant applicant)
    {
        return await _repository.Add(applicant);
    }

    public async Task<Applicant> GetApplicantByUserId(int userId)
    {
        return await _repository.GetById<Applicant>(userId);
    }
}