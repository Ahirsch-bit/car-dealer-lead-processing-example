using System.Text.Json.Serialization;

namespace CarDealer.LeadAutomation.Repository.DTOs;

public class ModelDTO
{
    [JsonPropertyName("model_id")]
    public string? ModelID { get; set; }
    [JsonIgnore]
    public string? Brand { get; set; }
    [JsonPropertyName("model_name")]
    public string? Model { get; set; }
    [JsonIgnore]
    public string? Year { get; set; }
    [JsonPropertyName("category")]
    public string? Category { get; set; }
    [JsonIgnore]
    public string? Engine { get; set; }
    [JsonPropertyName("price_range")]
    public string? PriceRange { get; set; }
    [JsonIgnore]
    public string? FuelEconomy { get; set; }
    [JsonIgnore]
    public string? Availability { get; set; }
    [JsonIgnore]
    public string? LeadTime { get; set; }
    [JsonIgnore]
    public string? Warranty { get; set; }
    [JsonIgnore]
    public string? Popular { get; set; }
    [JsonIgnore]
    public List<string> Features { get; set; } = new List<string>();
}
