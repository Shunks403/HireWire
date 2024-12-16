using AutoMapper;
using HireWireBackend.Core.Interfaces.IServices;
using HireWireBackend.Core.Models;
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
    private readonly ITagService _tagService;
    private readonly IJobVacancyTagService _jobVacancyTagService;
    private readonly IEmployerService _employerService;
    private readonly IMapper _mapper;

    public JobVacancyController(IJobVacancyService jobVacancyService, ITagService tagService , IJobVacancyTagService jobVacancyTagService , IEmployerService employerService,IMapper mapper)
    {
        _jobVacancyService = jobVacancyService;
        _mapper = mapper;
        _tagService = tagService;
        _jobVacancyTagService = jobVacancyTagService;
        _employerService = employerService;
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
    
    
    [HttpPost("createWithTags")]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> CreateJobVacancyWithTags(CreateVacancyWithTagsDTO createDto)
    {
        try
        {
            // Создание вакансии
            var jobVacancy = _mapper.Map<JobVacancy>(createDto.JobVacancy);
            await _jobVacancyService.Add(jobVacancy);

            // Создание или поиск тега
            foreach (var tagName in createDto.Tags)
            {
                var tag = await _tagService.GetOrCreateTagByNameAsync(tagName);
            
                // Связь вакансии и тега
                var jobVacancyTag = new JobVacancyTag
                {
                    VacancyId = jobVacancy.VacancyId,
                    TagId = tag.TagId
                };
                await _jobVacancyTagService.Add(jobVacancyTag);
            }

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    
    [HttpGet("userVacanciesWithTags")]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> GetUserVacanciesWithTags(int employerId)
    {
        try
        {
            var jobVacancies = _jobVacancyService.GetJobVacanciesEmployer(employerId);
            var employer = _employerService.FindById(employerId).Result;
            var vacanciesWithTags = jobVacancies.Select(vacancy => new JobVacancyCompactDTO
            {
                VacancyId = vacancy.VacancyId,
                Title = vacancy.Title,
                Description = vacancy.Description,
                Location = vacancy.Location,
                Status = vacancy.Status,
                CompanyName = employer.CompanyName,
                Salary = vacancy.Salary ?? 0,
                CreatedAt = vacancy.CreatedAt ?? DateTime.UtcNow,
                Tags = vacancy.JobVacancyTags.Select(tag => tag.Tag.Name).ToList()
            }).ToList();

            return Ok(vacanciesWithTags);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    
    [HttpGet("globalSearch")]
    [AllowAnonymous]
    public async Task<IActionResult> GlobalSearch(string keywords, string location, int page = 1, int pageSize = 10)
    {
        try
        {
            // Используем сервис для фильтрации и пагинации
            var result = await _jobVacancyService.GlobalSearch(keywords, location, page, pageSize);

            // Преобразуем вакансии в формат JobVacancyCompactDTO
            var jobs = result.Jobs.Select(vacancy => new JobVacancyCompactDTO
            {
                VacancyId = vacancy.VacancyId,
                Title = vacancy.Title,
                Status = vacancy.Status,
                CompanyName = vacancy.Employer?.CompanyName ?? "Unknown",
                Description = vacancy.Description,
                Location = vacancy.Location,
                Salary = vacancy.Salary ?? 0,
                CreatedAt = vacancy.CreatedAt ?? DateTime.UtcNow,
                Tags = vacancy.JobVacancyTags.Select(tag => tag.Tag.Name).ToList()
            }).ToList();

            return Ok(new
            {
                jobs,
                result.TotalPages
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    
}