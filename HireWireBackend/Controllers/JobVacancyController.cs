using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using HireWireBackend.Core.Interfaces.ILoggers;
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
    private readonly IBlobLogger _logger;

    public JobVacancyController(IJobVacancyService jobVacancyService, ITagService tagService , IJobVacancyTagService jobVacancyTagService , IEmployerService employerService,IMapper mapper , IBlobLogger logger)
    {
        _jobVacancyService = jobVacancyService;
        _mapper = mapper;
        _tagService = tagService;
        _jobVacancyTagService = jobVacancyTagService;
        _employerService = employerService;
        _logger = logger;
    }


    [HttpPost("creat")]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> CreateJobVacancy(JobVacancyDTO jobVacancyDto)
    {
        try
        {
            await _logger.LogAsync($"Attempting to create a new job vacancy: {jobVacancyDto.Title}", "INFO");

            await _jobVacancyService.Add(_mapper.Map<JobVacancy>(jobVacancyDto));

            await _logger.LogAsync($"Job vacancy '{jobVacancyDto.Title}' created successfully.", "INFO");
            return Ok();
        }
        catch (Exception ex)
        {
            await _logger.LogAsync($"Failed to create job vacancy '{jobVacancyDto?.Title}'. Exception: {ex.Message}", "ERROR");
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPut("update")]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> UpdateJobVacancy(JobVacancyDTO jobVacancyDto)
    {
        try
        {
            await _logger.LogAsync($"Attempting to update job vacancy with ID {jobVacancyDto.VacancyId}.", "INFO");

            await _jobVacancyService.Update(_mapper.Map<JobVacancy>(jobVacancyDto));

            await _logger.LogAsync($"Job vacancy with ID {jobVacancyDto.VacancyId} updated successfully.", "INFO");
            return Ok();
        }
        catch (Exception ex)
        {
            await _logger.LogAsync($"Failed to update job vacancy with ID {jobVacancyDto.VacancyId}. Exception: {ex.Message}", "ERROR");
            return BadRequest(ex.Message);
        }
    }
    
    
    [HttpGet("vacancies")]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> GetJobVacancyEmployer(int EmployerId)
    {
        try
        {
            await _logger.LogAsync($"Fetching job vacancies for employer with ID {EmployerId}.", "INFO");

            var listJobVacancy = _jobVacancyService.GetJobVacanciesEmployer(EmployerId);
            var mappedJobVacancies = _mapper.Map<List<JobVacancyDTO>>(listJobVacancy);

            await _logger.LogAsync($"Successfully fetched {mappedJobVacancies.Count} job vacancies for employer with ID {EmployerId}.", "INFO");

            return Ok(mappedJobVacancies);
        }
        catch (Exception ex)
        {
            await  _logger.LogAsync($"Failed to fetch job vacancies for employer with ID {EmployerId}. Exception: {ex.Message}", "ERROR");
            return BadRequest(ex.Message);
        }
    }
    
    
    
    [HttpDelete("delete")]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> DeleteJobVacancy(int id)
    {
        try
        {
            await _logger.LogAsync($"Attempting to delete job vacancy with ID {id}.", "INFO");

            await _jobVacancyService.Delete(id);

            await _logger.LogAsync($"Job vacancy with ID {id} deleted successfully.", "INFO");
            return Ok();
        }
        catch (Exception ex)
        {
            await _logger.LogAsync($"Failed to delete job vacancy with ID {id}. Exception: {ex.Message}", "ERROR");
            return BadRequest(ex.Message);
        }
    }
    
    
    [HttpPost("createWithTags")]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> CreateJobVacancyWithTags(CreateVacancyWithTagsDTO createDto)
    {
        try
        {
            await _logger.LogAsync($"Attempting to create a job vacancy with title '{createDto.JobVacancy.Title}' and tags.", "INFO");

            // Создание вакансии
            var jobVacancy = _mapper.Map<JobVacancy>(createDto.JobVacancy);
            await _jobVacancyService.Add(jobVacancy);
            _logger.LogAsync($"Job vacancy '{createDto.JobVacancy.Title}' created successfully with ID {jobVacancy.VacancyId}.", "INFO");

            // Создание или поиск тегов и привязка их к вакансии
            foreach (var tagName in createDto.Tags)
            {
                var tag = await _tagService.GetOrCreateTagByNameAsync(tagName);
                await _logger.LogAsync($"Tag '{tagName}' (ID {tag.TagId}) found or created successfully.", "INFO");

                var jobVacancyTag = new JobVacancyTag
                {
                    VacancyId = jobVacancy.VacancyId,
                    TagId = tag.TagId
                };
                await _jobVacancyTagService.Add(jobVacancyTag);
                await _logger.LogAsync($"Tag '{tagName}' linked to job vacancy with ID {jobVacancy.VacancyId}.", "INFO");
            }

            return Ok();
        }
        catch (Exception ex)
        {
            await _logger.LogAsync($"Failed to create job vacancy with title '{createDto?.JobVacancy?.Title}'. Exception: {ex.Message}", "ERROR");
            return BadRequest(ex.Message);
        }
    }
    
    
    [HttpGet("userVacanciesWithTags")]
    [Authorize(Roles = "Employer")]
    public async Task<IActionResult> GetUserVacanciesWithTags(int employerId)
    {
        try
        {
            await _logger.LogAsync($"Fetching job vacancies with tags for employer with ID {employerId}.", "INFO");

            var jobVacancies = _jobVacancyService.GetJobVacanciesEmployer(employerId);
            var employer = await _employerService.FindById(employerId);

            if (employer == null)
            {
                await _logger.LogAsync($"Employer with ID {employerId} not found.", "ERROR");
                return NotFound($"Employer with ID {employerId} not found.");
            }

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

            await _logger.LogAsync($"Successfully fetched {vacanciesWithTags.Count} vacancies for employer with ID {employerId}.", "INFO");

            return Ok(vacanciesWithTags);
        }
        catch (Exception ex)
        {
            await _logger.LogAsync($"Failed to fetch job vacancies for employer with ID {employerId}. Exception: {ex.Message}", "ERROR");
            return BadRequest(ex.Message);
        }
    }
    
    [HttpGet("globalSearch")]
    [AllowAnonymous]
    public async Task<IActionResult> GlobalSearch(string keywords, string location, int page = 1, int pageSize = 10)
    {
        try
        {
            await _logger.LogAsync($"Starting global search with keywords '{keywords}', location '{location}', page {page}, and pageSize {pageSize}.", "INFO");

            // Используем сервис для фильтрации и пагинации
            var result = await _jobVacancyService.GlobalSearch(keywords, location, page, pageSize);
            await _logger.LogAsync($"Global search returned {result.Jobs.Count()} jobs for keywords '{keywords}' and location '{location}'.", "INFO");

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

            await _logger.LogAsync($"Successfully processed global search results with {jobs.Count} jobs on page {page}.", "INFO");

            return Ok(new
            {
                jobs,
                result.TotalPages
            });
        }
        catch (Exception ex)
        {
            await _logger.LogAsync($"Global search failed with error: {ex.Message}", "ERROR");
            return BadRequest(ex.Message);
        }
    }
    
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetVacancyById(int id)
    {
        try
        {
            await _logger.LogAsync($"Fetching vacancy with ID {id}.", "INFO");

            var vacancy = await _jobVacancyService.FindById(id);
            if (vacancy == null)
            {
                await _logger.LogAsync($"Vacancy with ID {id} not found.", "ERROR");
                return NotFound("Vacancy not found.");
            }

            var employer = await _employerService.FindById(vacancy.Employer.EmployerId);
            if (employer == null)
            {
                await _logger.LogAsync($"Employer for vacancy ID {id} not found.", "ERROR");
                return NotFound("Employer not found.");
            }

            var vacancyWithTags = new JobVacancyCompactDTO
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
            };

            await _logger.LogAsync($"Successfully fetched vacancy with ID {id}.", "INFO");
            return Ok(vacancyWithTags);
        }
        catch (Exception ex)
        {
            await _logger.LogAsync($"Error fetching vacancy with ID {id}. Exception: {ex.Message}", "ERROR");
            return BadRequest(ex.Message);
        }
    }
    
}