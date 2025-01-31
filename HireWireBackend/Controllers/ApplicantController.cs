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
[Route("api/applicant")]
public class ApplicantController : Controller
{
    private readonly IApplicantService _applicantService;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IMapper _mapper;
    private readonly IBlobLogger _logger;

    public ApplicantController(IApplicantService applicantService, IMapper mapper , IBlobStorageService blobStorageService, IBlobLogger logger)
    {
        _applicantService = applicantService;
        _blobStorageService = blobStorageService;
        _mapper = mapper;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetApplicants()
    {
        try
        {
            await _logger.LogAsync("Fetching all applicants.", "INFO");

            // Попробуем получить всех кандидатов
            var applicants = _applicantService.GetAll();

            // Проверим, что данные не пустые
            if (applicants == null || !applicants.Any())
            {
                await _logger.LogAsync("No applicants found.", "INFO");
                return NotFound("No applicants found.");
            }

            // Попробуем выполнить маппинг
            List<ApplicantDTO> applicantDtos;
            try
            {
                applicantDtos = _mapper.Map<List<ApplicantDTO>>(applicants);
                await _logger.LogAsync($"Successfully mapped {applicantDtos.Count} applicants to DTOs.", "INFO");
            }
            catch (Exception ex)
            {
                _logger.LogAsync($"Error mapping applicants to DTOs. Exception: {ex.Message}", "ERROR");
                return StatusCode(500, "Error mapping applicants to DTOs.");
            }

            // Вернем результат
            await _logger.LogAsync($"Successfully retrieved {applicantDtos.Count} applicants.", "INFO");
            return Ok(applicantDtos);
        }
        catch (Exception ex)
        {
            await _logger.LogAsync($"An error occurred while retrieving applicants. Exception: {ex.Message}", "ERROR");
            return StatusCode(500, "An error occurred while retrieving applicants.");
        }
    }

   
    [HttpGet("{id}")]
    public async Task<IActionResult> GetApplicant(int id)
    {
        try
        {
            // Проверяем корректность входных данных
            if (id <= 0)
            {
                return BadRequest("Invalid applicant ID.");
            }

            // Пытаемся найти кандидата по ID
            var applicant = await _applicantService.FindById(id); 

            // Проверяем, найден ли кандидат
            if (applicant == null)
            {
                return NotFound($"Applicant with ID {id} not found.");
            }

            // Преобразуем найденного кандидата в DTO
            ApplicantDTO applicantDto;
            try
            {
                applicantDto = _mapper.Map<ApplicantDTO>(applicant);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Error mapping applicant to DTO.");
            }

            // Возвращаем успешный ответ с DTO
            // Логируем успешное завершение
            await _logger.LogAsync($"Successfully retrieved applicant with ID {id}.","INFO");
            return Ok(applicantDto);
        }
        catch (Exception ex)
        {
            // Логируем успешное завершение
            await _logger.LogAsync($"Successfully retrieved applicant with ID {id}.","INFO");
            return StatusCode(500, "An error occurred while retrieving the applicant.");
        }
    }
    
   
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteApplicant(int id)
    {
        try
        {
            // Проверяем корректность входного параметра
            if (id <= 0)
            {
                return BadRequest("Invalid applicant ID.");
            }

            // Пытаемся найти кандидата по ID
            var applicant = await _applicantService.FindById(id); 

            // Если кандидат не найден, возвращаем 404
            if (applicant == null)
            {
                return NotFound($"Applicant with ID {id} not found.");
            }

            // Удаляем кандидата
            try
            {
                await _applicantService.Delete(applicant.ApplicantId); 
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, $"An error occurred while deleting the applicant with ID {id}.");
            }

            // Возвращаем статус 204 (No Content), если удаление прошло успешно
            return NoContent();
        }
        catch (Exception ex)
        {
           
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

   
  [HttpPost("create-profile")]
  public async Task<IActionResult> CreateProfile([FromForm] ApplicantDTO profileDto)
{
    try
    {
        // Проверка наличия userId в токене
        var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            await _logger.LogAsync("Unauthorized access attempt: User ID not found in token.","ERROR");
            return Unauthorized("User ID not found in token.");
        }

        // Попытка преобразовать userId в int
        if (!int.TryParse(userIdClaim.Value, out int userId))
        {
            await _logger.LogAsync("Invalid User ID format in token.","ERROR");
            return BadRequest("Invalid User ID in token.");
        }

        // Проверка на наличие файла
        if (profileDto.File == null || profileDto.File.Length == 0)
        {
            await _logger.LogAsync($"User ID {userId}: Attempt to create profile without resume file.","ERROR");
            return BadRequest("Resume file is required.");
        }

        string resumeUrl;
        try
        {
            // Загружаем файл в Azure Blob Storage
            resumeUrl = await _blobStorageService.UploadFileAsync(profileDto.File);
        }
        catch (Exception ex)
        {
            // Логируем ошибку загрузки файла
            await _logger.LogAsync($"User ID {userId}: Error uploading resume file. Exception: {ex.Message}","ERROR");
            return StatusCode(500, "Error uploading resume file.");
        }

        // Создаем объект Applicant
        var applicant = new Applicant
        {
            ApplicantId = userId,
            Resume = resumeUrl,
            Skills = profileDto.Skills,
            Education = profileDto.Education,
            CreatedAt = DateTime.UtcNow
        };

        Applicant createdApplicant;
        try
        {
            // Сохраняем данные в базу
            createdApplicant = await _applicantService.CreateApplicant(applicant); 
        }
        catch (Exception ex)
        {
            // Логируем ошибку создания профиля
            await _logger.LogAsync($"User ID {userId}: Error creating applicant profile. Exception: {ex.Message}","ERROR");
            return StatusCode(500, "An error occurred while creating the applicant profile.");
        }

        // Логируем успешное создание профиля
        await _logger.LogAsync($"User ID {userId}: Applicant profile created successfully with ID {createdApplicant.ApplicantId}.","INFO");

        // Возвращаем успешный ответ
        return Ok(new
        {
            message = "Applicant profile created successfully.",
            applicantId = createdApplicant.ApplicantId
        });
    }
    catch (Exception ex)
    {
        // Логируем общую ошибку
        await _logger.LogAsync($"Unexpected error in CreateProfile. Exception: {ex.Message}","ERROR");
        return StatusCode(500, "An unexpected error occurred while processing your request.");
    }
}
    
    
    [HttpPut("update-profile/{id}")]
public async Task<IActionResult> UpdateProfile(int id, [FromForm] ApplicantDTO profileDto)
{
    try
    {
        // Логируем начало обновления профиля
        await _logger.LogAsync($"Start updating profile for Applicant ID {id}.","INFO");

        // Ищем кандидата в базе
        var applicant = await _applicantService.FindById(id);
        if (applicant == null)
        {
            await _logger.LogAsync($"Applicant ID {id} not found.","ERROR");
            return NotFound(new { message = "Applicant not found." });
        }

        // Если передан новый файл резюме
        if (profileDto.File != null && profileDto.File.Length > 0)
        {
            try
            {
                // Удаляем старое резюме, если оно существует
                if (!string.IsNullOrEmpty(applicant.Resume))
                {
                    await _logger.LogAsync($"Deleting old resume for Applicant ID {id}.","INFO");
                    await _blobStorageService.DeleteFileAsync(applicant.Resume);
                }

                // Загружаем новое резюме
                await _logger.LogAsync($"Uploading new resume for Applicant ID {id}.","INFO");
                var resumeUrl = await _blobStorageService.UploadFileAsync(profileDto.File);
                applicant.Resume = resumeUrl;
            }
            catch (Exception ex)
            {
                // Логируем ошибку загрузки нового резюме
                await _logger.LogAsync($"Error uploading new resume for Applicant ID {id}. Exception: {ex.Message}","ERROR");
                return StatusCode(500, new { message = "Failed to upload new resume.", details = ex.Message });
            }
        }

        // Обновляем остальные данные кандидата
        applicant.Skills = profileDto.Skills;
        applicant.Education = profileDto.Education;

        try
        {
            // Обновляем запись в базе
            await _applicantService.Update(applicant);
            await _logger.LogAsync($"Applicant ID {id} successfully updated.","INFO");
        }
        catch (Exception ex)
        {
            // Логируем ошибку обновления записи
            await _logger.LogAsync($"Error updating Applicant ID {id}. Exception: {ex.Message}","ERROR");
            return StatusCode(500, new { message = "Failed to update applicant data.", details = ex.Message });
        }

        // Успешное завершение обновления
        return Ok(new { message = "Applicant profile updated successfully." });
    }
    catch (Exception ex)
    {
        // Логируем общую ошибку
        await _logger.LogAsync($"Unexpected error while updating Applicant ID {id}. Exception: {ex.Message}","ERROR");
        return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
    }
}
    
}