namespace HireWireBackend.Core.Interfaces.IServices;

public interface IApplicantService : IBaseService<Applicant>
{
    public Task<Applicant> Add(Applicant applicant);
    
    Task<Applicant> CreateApplicant(Applicant applicant);
    Task<Applicant> GetApplicantByUserId(int userId);
}