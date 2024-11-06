namespace HireWireBackend.Core.Interfaces.IServices;

public interface IJobApplicationService : IBaseService<JobApplication>
{
    public Task<JobApplication> Add(JobApplication jobApplication);
}