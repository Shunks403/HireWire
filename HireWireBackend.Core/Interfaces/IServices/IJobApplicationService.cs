namespace HireWireBackend.Core.Interfaces.IServices;

public interface IJobApplicationService : IBaseService<JobApplication>
{
    public Task<JobApplication> Add(JobApplication jobApplication);

    public  Task<int> GetResponsesCountForEmployer(int employerId);

    public Task<IEnumerable<JobApplication>> GetResponsesForEmployer(int employerId, int page, int pageSize);

}