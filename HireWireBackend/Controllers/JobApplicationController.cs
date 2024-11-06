using AutoMapper;
using HireWireBackend.Core.Interfaces.IServices;
using HireWireBackend.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireWireBackend.Controllers;


[Authorize]
[ApiController]
[Route("api/jobApplication")]
public class JobApplicationController : Controller
{
    private readonly IJobApplicationService _jobApplicationService;
    private readonly IMapper _mapper;

    public JobApplicationController(IJobApplicationService jobApplicationService, IMapper mapper)
    {
        _jobApplicationService = jobApplicationService;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetJobApplications()
    {
        var jobApplications = _jobApplicationService.GetAll();
        var jobApplicationDtos = _mapper.Map<List<JobApplicationDTO>>(jobApplications);
        return Ok(jobApplicationDtos);
    }

    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetJobApplication(int id)
    {
        var jobApplication = await _jobApplicationService.FindById(id);
        if (jobApplication == null)
            return NotFound();

        var jobApplicationDto = _mapper.Map<JobApplicationDTO>(jobApplication);
        return Ok(jobApplicationDto);
    }

   
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateJobApplication(int id, JobApplicationDTO jobApplicationDto)
    {
        if (id != jobApplicationDto.ApplicationId)
            return BadRequest();

        var jobApplication = _mapper.Map<JobApplication>(jobApplicationDto);
        await _jobApplicationService.Update(jobApplication);

        return NoContent();
    }

    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJobApplication(int id)
    {
        var jobApplication = await _jobApplicationService.FindById(id);
        if (jobApplication == null)
            return NotFound();

        _jobApplicationService.Delete(jobApplication.ApplicationId);

        return NoContent();
    }

    
    [HttpPost]
    public async Task<IActionResult> CreateJobApplication(JobApplicationDTO jobApplicationDto)
    {
        await _jobApplicationService.Add(_mapper.Map<JobApplication>(jobApplicationDto));
        return Ok();
    }
    
    
    
}