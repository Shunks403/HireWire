namespace HireWireBackend.Core.Interfaces.IServices;

public interface IEmployerService : IBaseService<Employer>
{
    public Task<Employer> Add(Employer employer);
}