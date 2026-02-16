using CarDealer.LeadAutomation.Contracts;
using CarDealer.LeadAutomation.Repository.Interfaces;
using CarDealer.LeadAutomation.Services.Interfaces;
using CarDealer.LeadAutomation.Services.LeadEnrichment;
using CarDealer.LeadAutomation.Services.LeadEnrichment.Extensions;

namespace CarDealer.LeadAutomation.Services;

public class LeadsSerivce: ILeadsSerivice
{
    private readonly ILeadEnrichmentRequest _leadEnrichmentRequest;
    private readonly IBranchRepository _branchRepository;
    private readonly IModelRepository _modelRepository;
    public LeadsSerivce(ILeadEnrichmentRequest leadEnrichmentRequest, IBranchRepository branchRepository, 
        IModelRepository modelRepository)
    {
        _leadEnrichmentRequest= leadEnrichmentRequest;
        _branchRepository= branchRepository;
        _modelRepository= modelRepository;
    }

    public async Task<ProcessedLeadDTO> ProcessLeadAsync(LeadRequest leadRequest)
    {
        var branchData = _branchRepository.GetBranchById(int.Parse(leadRequest.BranchID));
        var modelData = _modelRepository.GetModelById(leadRequest.AskedCar);

        var xenrichmentRequest = new EnrichmentRequest
        {
            Email = leadRequest.Email,
            Phone = leadRequest.Phone,
            Area = leadRequest.Area,
        };
        var enrichmentData = await _leadEnrichmentRequest.EnrichLeadAsync(xenrichmentRequest);

        var score = enrichmentData.GetEnrichmentScore() + modelData.GetCarScore() > 100
            ? 100
            : enrichmentData.GetEnrichmentScore() + modelData.GetCarScore();
        return new ProcessedLeadDTO()
        {
            OriginalLead = leadRequest,
            BranchInfo = branchData,
            CarInfo = modelData,
            Enrichment = new ProcessedLeadEnrichment()
            {
                Geographic = new ProcessedLeadGeographicEnrichment()
                {
                    City = enrichmentData.Data.Geographic.City,
                    Region = enrichmentData.Data.Geographic.Region
                },
                EmailInsights = new ProcessedLeadEmailInsights()
                {
                    TrustLevel = enrichmentData.Data.EmailInsights.TrustLevel
                },
                PhoneInsights = new ProcessedLeadPhoneInsights()
                {
                    Carrier = enrichmentData.Data.PhoneInsights.Carrier,
                    Verified = enrichmentData.Data.PhoneInsights.Verified
                },
                LeadPriority = enrichmentData.Data.LeadPriority
            },
            Score = score,
            Priority = score switch
            {
                >= 70 => LeadPriority.HOT,
                >= 40 => LeadPriority.WARM,
                _ => LeadPriority.COLD
            },
            AssignedTo = score switch
            {
                >= 70 => branchData.Manager,
                >= 40 => $"Sales Rep {leadRequest.WorkerCode}",
                _ => "General Pool"
            },
            Status = "Processed"
        };

    }
}