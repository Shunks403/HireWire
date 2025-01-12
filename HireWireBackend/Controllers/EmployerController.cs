using AutoMapper;
using HireWireBackend.Core.Interfaces.ILoggers;
using HireWireBackend.Core.Interfaces.IServices;
using HireWireBackend.DTO;
using LibraryManegerBackend.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HireWireBackend.Controllers;

[Authorize]
[ApiController]
[Route("api/employer")]
public class EmployerController : Controller
{
    private readonly IEmployerService _employerService;
    private readonly IMapper _mapper;
    private readonly IBlobLogger _logger;
    public EmployerController(IEmployerService employerService, IMapper mapper, IBlobLogger logger)
    {
        _employerService = employerService;
        _mapper = mapper;
        _logger = logger;
    }
    
    
    [HttpGet]
    public async Task<IActionResult> GetEmployers()
    {
        try
        {
            await _logger.LogAsync("Fetching all employers.", "INFO");

            var employers = _employerService.GetAll();
            var employerDtos = _mapper.Map<List<EmployerDTO>>(employers);

            await _logger.LogAsync($"Successfully fetched {employerDtos.Count} employers.", "INFO");

            return Ok(employerDtos);
        }
        catch (Exception ex)
        {
            await _logger.LogAsync($"Failed to fetch employers. Exception: {ex.Message}", "ERROR");
            return BadRequest("Failed to fetch employers.");
        }
    }
    
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployer(int id)
    {
        try
        {
            await _logger.LogAsync($"Fetching employer with ID {id}.", "INFO");

            var employer = await _employerService.FindById(id);
            if (employer == null)
            {
                await _logger.LogAsync($"Employer with ID {id} not found.", "ERROR");
                return NotFound("Employer not found.");
            }

            var employerDto = _mapper.Map<EmployerDTO>(employer);
            await _logger.LogAsync($"Successfully fetched employer with ID {id}.", "INFO");

            return Ok(employerDto);
        }
        catch (Exception ex)
        {
            await _logger.LogAsync($"Failed to fetch employer with ID {id}. Exception: {ex.Message}", "ERROR");
            return BadRequest("Failed to fetch employer.");
        }
    }
    
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployer(int id, EmployerDTO employerDto)
    {
        try
        {
            await _logger.LogAsync($"Attempting to update employer with ID {id}.", "INFO");

            if (id != employerDto.EmployerId)
            {
                await _logger.LogAsync($"Mismatch in provided IDs: Path ID {id}, Employer DTO ID {employerDto.EmployerId}.", "ERROR");
                return BadRequest("The provided ID does not match the Employer ID in the DTO.");
            }

            var employer = _mapper.Map<Employer>(employerDto);

            await _employerService.Update(employer);

            await _logger.LogAsync($"Employer with ID {id} updated successfully.", "INFO");
            return NoContent();
        }
        catch (Exception ex)
        {
            await _logger.LogAsync($"Failed to update employer with ID {id}. Exception: {ex.Message}", "ERROR");
            return BadRequest("Failed to update employer.");
        }
    }
    
    [HttpGet("check-profile/{id}")]
    public async Task<IActionResult> CheckEmployerProfile(int id)
    {
        try
        {
            await _logger.LogAsync($"Checking profile existence for employer with ID {id}.", "INFO");

            // Ищем работодателя по ID
            var employer = await _employerService.FindById(id);

            // Если работодатель не найден, возвращаем false
            if (employer == null)
            {
                await _logger.LogAsync($"Employer with ID {id} does not have a profile.", "INFO");
                return Ok(new { hasProfile = false });
            }

            // Если работодатель найден, возвращаем true
            await _logger.LogAsync($"Employer with ID {id} has a profile.", "INFO");
            return Ok(new { hasProfile = true });
        }
        catch (Exception ex)
        {
            await _logger.LogAsync($"Error checking profile for employer with ID {id}. Exception: {ex.Message}", "ERROR");
            return BadRequest("Failed to check employer profile.");
        }
    }
    
    

    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployer(int id)
    {
        try
        {
            await _logger.LogAsync($"Attempting to delete employer with ID {id}.", "INFO");

            var employer = await _employerService.FindById(id);
            if (employer == null)
            {
                await  _logger.LogAsync($"Employer with ID {id} not found.", "ERROR");
                return NotFound("Employer not found.");
            }

            await _employerService.Delete(employer.EmployerId);

            await _logger.LogAsync($"Employer with ID {id} deleted successfully.", "INFO");
            return NoContent();
        }
        catch (Exception ex)
        {
            await _logger.LogAsync($"Failed to delete employer with ID {id}. Exception: {ex.Message}", "ERROR");
            return BadRequest("Failed to delete employer.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmployer(EmployerDTO employerDto)
    {
        try
        {
            await _logger.LogAsync($"Attempting to create employer with name '{employerDto.CompanyName}'.", "INFO");

            await _employerService.Add(_mapper.Map<Employer>(employerDto));

            await _logger.LogAsync($"Employer '{employerDto.CompanyName}' created successfully.", "INFO");
            return Ok();
        }
        catch (Exception ex)
        {
            await _logger.LogAsync($"Failed to create employer '{employerDto?.CompanyName}'. Exception: {ex.Message}", "ERROR");
            return BadRequest("Failed to create employer.");
        }
    }
    
    
    
    
}