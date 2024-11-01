using HireWireBackend.Core.Interfaces.IServices;
using LibraryManegerBackend.Core.Interfaces;

namespace HireWireBackend.Core.Services;

public class EmployerService : IEmployerService
{
    private readonly IRepository _repository;

    public EmployerService(IRepository repository)
    {
        _repository = repository;
    }
    
    public Task<Employer> Update(Employer entity)
    {
        return _repository.Update(entity);
    }

    public Task<Employer> FindById(int id)
    {
        return _repository.GetById<Employer>(id);
    }

    public Task Delete(int id)
    {
        return _repository.Delete<Employer>(id);
    }

    public IEnumerable<Employer> GetAll()
    {
        return _repository.GetAll<Employer>();
    }
}