using CarDealer.LeadAutomation.Contracts;

namespace CarDealer.LeadAutomation.Services.Interfaces;

public interface ILeadEnrichmentRequest
{
    Task<EnrichmentResponse?> EnrichLeadAsync(EnrichmentRequest request);
}