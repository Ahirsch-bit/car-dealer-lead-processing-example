using CarDealer.LeadAutomation.Contracts;

namespace CarDealer.LeadAutomation.Services.Interfaces;

public interface ILeadsSerivice
{
    Task<ProcessedLeadDTO> ProcessLeadAsync(LeadRequest leadRequest, CancellationToken ctx);
}