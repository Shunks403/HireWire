using AutoMapper;
using HireWireBackend.Core.Interfaces.IServices;
using HireWireBackend.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireWireBackend.Controllers;


[Authorize]
[ApiController]
[Route("api/employer")]
public class ApplicantController : Controller
{
    private readonly IApplicantService _applicantService;
    private readonly IMapper _mapper;

    public ApplicantController(IApplicantService applicantService, IMapper mapper)
    {
        _applicantService = applicantService;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetApplicants()
    {
        var applicants = _applicantService.GetAll();
        var applicantDtos = _mapper.Map<List<ApplicantDTO>>(applicants);
        return Ok(applicantDtos);
    }

   
    [HttpGet("{id}")]
    public async Task<IActionResult> GetApplicant(int id)
    {
        var applicant = await _applicantService.FindById(id);
        if (applicant == null)
            return NotFound();

        var applicantDto = _mapper.Map<ApplicantDTO>(applicant);
        return Ok(applicantDto);
    }

   
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateApplicant(int id, ApplicantDTO applicantDto)
    {
        if (id != applicantDto.ApplicantId)
            return BadRequest();

        var applicant = _mapper.Map<Applicant>(applicantDto);
        await _applicantService.Update(applicant);

        return NoContent();
    }

   
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteApplicant(int id)
    {
        var applicant = await _applicantService.FindById(id);
        if (applicant == null)
            return NotFound();

        _applicantService.Delete(applicant.ApplicantId);

        return NoContent();
    }

   
    [HttpPost]
    public async Task<IActionResult> CreateApplicant(ApplicantDTO applicantDto)
    {
        await _applicantService.Add(_mapper.Map<Applicant>(applicantDto));
        return Ok();
    }
    
}