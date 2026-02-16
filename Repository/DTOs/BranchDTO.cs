using System.Text.Json.Serialization;

namespace CarDealer.LeadAutomation.Repository.DTOs;

public class BranchDTO
{
    [JsonPropertyName("branch_id")]
    public int BranchID { get; set; }
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("region")]
    public string? Region { get; set; }
    [JsonIgnore]
    public string? City { get; set; }
    [JsonIgnore]
    public string? Address { get; set; }
    [JsonPropertyName("manager")]
    public string? Manager { get; set; }
    [JsonIgnore]
    public string? Email { get; set; }
    [JsonIgnore]
    public string? Phone { get; set; }
    [JsonIgnore]
    public string? WorkingHours { get; set; }
    [JsonIgnore]
    public string? Specialties { get; set; }
    [JsonIgnore]
    public string? Languages { get; set; }
}
