using CarDealer.LeadAutomation.Contracts;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CarDealer.LeadAutomation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeadsController : ControllerBase
{
    private readonly IValidator<LeadRequest> _leadValidator;

    public LeadsController(IValidator<LeadRequest> leadValidator)
    {
        _leadValidator = leadValidator;
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
        
        //TODO: Process the lead (e.g., save to database, send notifications, etc.)
        return Ok(new {status="Lead processed successfully"});
    }
}