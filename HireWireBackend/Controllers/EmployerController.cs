using AutoMapper;
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

    public EmployerController(IEmployerService employerService, IMapper mapper)
    {
        _employerService = employerService;
        _mapper = mapper;
    }
    
    
    [HttpGet]
    public async Task<IActionResult> GetEmployers()
    {
        var employers =  _employerService.GetAll();
        var employerDtos =  _mapper.Map<List<EmployerDTO>>(employers);
        return Ok(employerDtos);
    }
    
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployer(int id)
    {
        var employer = await _employerService.FindById(id);
        if (employer == null)
            return NotFound();

        var employerDto = _mapper.Map<EmployerDTO>(employer); 
        return Ok(employerDto);
    }
    
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployer(int id, EmployerDTO employerDto)
    {
        if (id != employerDto.EmployerId)
            return BadRequest();

        var employer = _mapper.Map<Employer>(employerDto); 
        
        await _employerService.Update(employer);
        
        return NoContent();
    }

    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployer(int id)
    {
        var employer = await _employerService.FindById(id);
        if (employer == null)
            return NotFound();

        _employerService.Delete(employer.EmployerId);

        return NoContent();
    }
    
    
}