using CarDealer.LeadAutomation.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CarDealer.LeadAutomation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeadsQueueController: ControllerBase
{
    private readonly ILeadProcessingQueue _leadProcessingQueue;
    
    public LeadsQueueController(ILeadProcessingQueue leadProcessingQueue)
    {
        _leadProcessingQueue = leadProcessingQueue;
    }
    
    [HttpPost("cancel")]
    public IActionResult Cancel(Guid taskId)
    {
        var canceled = _leadProcessingQueue.CancelLeadProcessing(taskId);
        return canceled ? Ok() : NotFound();
    }
    
    [HttpGet("status")]
    public IActionResult Status(Guid taskId)
    {

        var status = _leadProcessingQueue.GetTaskStatus(taskId);

        return Ok(status);

    }
}