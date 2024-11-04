using AutoMapper;
using HireWireBackend.Core.Interfaces.IServices;
using HireWireBackend.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireWireBackend.Controllers;

[Authorize]
[ApiController]
[Route("api/vacancy")]
public class JobVacancyController : Controller
{
    private readonly IJobVacancyService _jobVacancyService;
    private readonly IMapper _mapper;

    public JobVacancyController(IJobVacancyService jobVacancyService, IMapper mapper)
    {
        _jobVacancyService = jobVacancyService;
        _mapper = mapper;
    }


    [HttpPost("creat")]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> CreateJobVacancy(JobVacancyDTO jobVacancyDto)
    {
        try
        {
            await _jobVacancyService.Add(_mapper.Map<JobVacancy>(jobVacancyDto));
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        
    }
    
    [HttpPut("update")]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> UpdateJobVacancy(JobVacancyDTO jobVacancyDto)
    {
        try
        {
            await _jobVacancyService.Update(_mapper.Map<JobVacancy>(jobVacancyDto));
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        
    }
    
    
    [HttpGet("vacancies")]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> GetJobVacancyEmployer(int EmployerId)
    {
        try
        {
            var listJobVacancy =  _jobVacancyService.GetJobVacanciesEmployer(EmployerId);
            return Ok(_mapper.Map<List<JobVacancyDTO>>(listJobVacancy));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        
    }
    
    
    
    [HttpDelete("delete")]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> DeleteJobVacancy(int id)
    {
        try
        {
            await _jobVacancyService.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        
    }
    
    
    
}