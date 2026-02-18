using CarDealer.LeadAutomation.Contracts;

namespace CarDealer.LeadAutomation.Repository.Interfaces;

public interface ILeadsStore
{
    void AddLead(ProcessedLeadDTO leadDto);
    ProcessedLeadDTO? GetLeadByEmail(string email);
    ProcessedLeadDTO? GetLeadByPhone(string id);
    public IEnumerable<ProcessedLeadDTO> GetAllLeads();
}