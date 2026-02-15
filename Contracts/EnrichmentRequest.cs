using System.Text.Json.Serialization;

namespace CarDealer.LeadAutomation.Contracts;

public class EnrichmentRequest
{
    [JsonPropertyName("email")]
    public string Email { get; set; }
    [JsonPropertyName("phone")]
    public string Phone { get; set; }
    [JsonPropertyName("area")]
    public string Area { get; set; }
}