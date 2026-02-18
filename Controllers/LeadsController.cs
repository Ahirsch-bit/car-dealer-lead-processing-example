using CarDealer.LeadAutomation.Contracts;
using CarDealer.LeadAutomation.Repository.Interfaces;
using CarDealer.LeadAutomation.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CarDealer.LeadAutomation.Controllers;

[ApiController]
[Route("api/")]
public class LeadsController : ControllerBase
{
    private readonly IValidator<LeadRequest> _leadValidator;
    private readonly ILeadsSerivice _leadsService;
    private readonly ILeadsStore _leadsStore;
    private readonly ILeadProcessingQueue _leadProcessingQueue;

    public LeadsController(ILeadProcessingQueue queue,IValidator<LeadRequest> leadValidator, ILeadsSerivice leadsService,
        ILeadsStore leadsStore)
    {
        _leadProcessingQueue = queue;
        _leadValidator = leadValidator;
        _leadsService = leadsService;
        _leadsStore = leadsStore;
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

        var taskId = _leadProcessingQueue.QueueLead(async token =>
        {
            var processedLead = await _leadsService.ProcessLeadAsync(request, token);
            _leadsStore.AddLead(processedLead);
        });
        return Accepted(new
        {
            TaskId = taskId,
            Request = request
        });
    }

    [HttpGet]
    [Route("leads")]
    public IActionResult GetLeads(string? email, string? phone)
    {
        // If both email and phone are provided, prioritize email lookup
        if (email != null)
        {
            var lead = _leadsStore.GetLeadByEmail(email);
            return Ok(lead);
        }

        // If only phone is provided, lookup by phone
        if (phone != null)
        {
            var lead = _leadsStore.GetLeadByPhone(phone);
            return Ok(lead);
        }

        // If neither is provided, return all leads
        var leads = _leadsStore.GetAllLeads();
        return Ok(leads);
    }
}