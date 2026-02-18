using CarDealer.LeadAutomation.Contracts;
using CarDealer.LeadAutomation.Repository.Interfaces;

namespace CarDealer.LeadAutomation.Repository;

public class LeadStoreDb:ILeadsStore
{
    private readonly Dictionary<Guid, ProcessedLeadDTO> _leads = new();

    public void AddLead(ProcessedLeadDTO leadDto)
    {
        var id = Guid.NewGuid();
        _leads[id]= leadDto;
    }

    public ProcessedLeadDTO? GetLeadByEmail(string email)
    {
        return _leads.Values.FirstOrDefault(l =>
            l.OriginalLead.Email == email);
    }

    public ProcessedLeadDTO? GetLeadByPhone(string phone)
    {
        return _leads.Values.FirstOrDefault(l =>
            l.OriginalLead.Phone == phone);
    }

    public IEnumerable<ProcessedLeadDTO> GetAllLeads()
    {
        return _leads.Values;
    }
    
}