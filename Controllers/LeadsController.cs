using CarDealer.LeadAutomation.Contracts;
using CarDealer.LeadAutomation.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CarDealer.LeadAutomation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeadsController : ControllerBase
{
    private readonly IValidator<LeadRequest> _leadValidator;
    private readonly ILeadsSerivice _leadsService;

    public LeadsController(IValidator<LeadRequest> leadValidator, ILeadsSerivice leadsService)
    {
        _leadValidator = leadValidator;
        _leadsService = leadsService;
    }

    [HttpPost]
    [Route("leads")]
    public async Task<IActionResult> ProcessLead([FromBody] LeadRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var validationResult = await _leadValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var processedLead =_leadsService.ProcessLeadAsync(request);
        return Ok(processedLead);
    }
}