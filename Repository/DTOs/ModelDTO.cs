namespace CarDealer.LeadAutomation.Repository.DTOs;

public class ModelDTO
{
    public string? ModelID { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? Year { get; set; }
    public string? Category { get; set; }
    public string? Engine { get; set; }
    public string? PriceRange { get; set; }
    public string? FuelEconomy { get; set; }
    public string? Availability { get; set; }
    public string? LeadTime { get; set; }
    public string? Warranty { get; set; }
    public string? Popular { get; set; }
    public List<string> Features { get; set; } = new List<string>();
}

