using System.Text.Json.Serialization;
using CarDealer.LeadAutomation.Repository.DTOs;

namespace CarDealer.LeadAutomation.Contracts;

public class ProcessedLeadDTO
{
    [JsonPropertyName("original_lead")]
    public LeadRequest OriginalLead { get; set; }
    [JsonPropertyName("branch_info")]
    public BranchDTO BranchInfo { get; set; }
    [JsonPropertyName("car_info")]
    public ModelDTO? CarInfo { get; set; }
    [JsonPropertyName("enrichment")]
    public ProcessedLeadEnrichment Enrichment { get; set; }
    [JsonPropertyName("score")]
    public int Score { get; set; }
    [JsonPropertyName("priority")]
    public LeadPriority Priority { get; set; }
    [JsonPropertyName("assigned_to")]
    public string? AssignedTo { get; set; }
    [JsonPropertyName("status")]
    public string Status { get; set; }
}

public class ProcessedLeadEnrichment
{
    [JsonPropertyName("geographic")]
    public ProcessedLeadGeographicEnrichment Geographic { get; set; }
  
    [JsonPropertyName("email_insights")]
    public ProcessedLeadEmailInsights EmailInsights { get; set; }
  
    [JsonPropertyName("phone_insights")]
    public ProcessedLeadPhoneInsights PhoneInsights { get; set; }
  
    [JsonPropertyName("lead_priority")]
    public string LeadPriority { get; set; }
}
public class ProcessedLeadGeographicEnrichment
{
    [JsonPropertyName("city")]
    public string City { get; set; }
  
    [JsonPropertyName("region")]
    public string Region { get; set; }
}

public class ProcessedLeadEmailInsights
{
    [JsonPropertyName("trust_level")]
    public ProcessedLeadTrustLevel TrustLevel { get; set; }
}
public enum ProcessedLeadTrustLevel
{
    High,
    Medium,
    Low
}

public class ProcessedLeadPhoneInsights
{
    [JsonPropertyName("carrier")]
    public string Carrier { get; set; }
  
    [JsonPropertyName("verified")]
    public bool Verified { get; set; }
}

public enum LeadPriority
{
    HOT,
    WARM,
    COLD
}