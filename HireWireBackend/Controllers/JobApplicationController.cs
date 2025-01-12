using System.Security.Claims;
using AutoMapper;
using HireWireBackend.Core.Interfaces.ILoggers;
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
    private readonly IBlobLogger _logger;
    public JobApplicationController(IJobApplicationService jobApplicationService, IMapper mapper , IBlobLogger logger)
    {
        _jobApplicationService = jobApplicationService;
        _mapper = mapper;
        _logger = logger;
    }
    
    [HttpPost("apply")]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> ApplyToVacancy([FromBody] JobApplicationDTO applicationDto)
    {
        try
        {
            // Проверка VacancyId
            if (applicationDto.VacancyId == null)
            {
                _logger.LogAsync("Application submission failed: Vacancy ID is missing.", "ERROR");
                return BadRequest("Vacancy ID is required.");
            }

            // Получение User ID из токена
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                _logger.LogAsync("Application submission failed: User ID not found in token.", "ERROR");
                return Unauthorized("User ID not found in token.");
            }

            if (!int.TryParse(userIdClaim.Value, out int applicantId))
            {
                _logger.LogAsync("Application submission failed: Invalid User ID format.", "ERROR");
                return BadRequest("Invalid User ID format in token.");
            }

            // Создание нового объекта JobApplication
            var jobApplication = new JobApplication
            {
                VacancyId = applicationDto.VacancyId,
                ApplicantId = applicantId,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
            };

            // Добавление заявки
            await _jobApplicationService.Add(jobApplication);
            _logger.LogAsync($"Application submitted successfully for Vacancy ID {applicationDto.VacancyId} by User ID {applicantId}.", "INFO");

            return Ok(new { message = "Application submitted successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogAsync($"Application submission failed with error: {ex.Message}", "ERROR");
            return StatusCode(500, new { message = "Failed to submit application.", error = ex.Message });
        }
    }


   
    [HttpGet("responses")]
    [Authorize(Roles = "Employer")]
   public async Task<IActionResult> GetResponsesForEmployer([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
{
    try
    {
        _logger.LogAsync("Fetching responses for employer started.", "INFO");

        var employerIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (employerIdClaim == null)
        {
            _logger.LogAsync("Unauthorized access: Employer ID not found in token.", "ERROR");
            return Unauthorized("Employer ID not found in token.");
        }

        int employerId = int.Parse(employerIdClaim.Value);

        _logger.LogAsync($"Fetching responses for employer with ID {employerId}. Page: {page}, PageSize: {pageSize}.", "INFO");

        // Получаем общее количество откликов
        var totalResponses = await _jobApplicationService.GetResponsesCountForEmployer(employerId);
        _logger.LogAsync($"Total responses for employer with ID {employerId}: {totalResponses}.", "INFO");

        // Получаем отклики с учетом пагинации
        var jobApplications = await _jobApplicationService.GetResponsesForEmployer(employerId, page, pageSize);
        _logger.LogAsync($"Fetched {jobApplications.Count()} responses for employer with ID {employerId}.", "INFO");

        var response = jobApplications.Select(app => new
        {
            ApplicationId = app.ApplicationId,
            VacancyTitle = app.Vacancy.Title,
            ApplicantName = $"{app.Applicant.ApplicantNavigation.FirstName} {app.Applicant.ApplicantNavigation.LastName}",
            ResumeUrl = app.Applicant.Resume
        });

        _logger.LogAsync($"Successfully fetched responses for employer with ID {employerId}.", "INFO");

        return Ok(new
        {
            TotalPages = (int)Math.Ceiling(totalResponses / (double)pageSize),
            CurrentPage = page,
            Responses = response
        });
    }
    catch (Exception ex)
    {
        _logger.LogAsync($"Failed to fetch responses for employer. Exception: {ex.Message}", "ERROR");
        return BadRequest(new { message = "Failed to fetch responses.", error = ex.Message });
    }
}

    
}