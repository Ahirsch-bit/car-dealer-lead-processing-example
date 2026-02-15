namespace CarDealer.LeadAutomation.Contracts;

public class EnrichmentResponse
{
    public bool Enriched { get; set; }
    public EnrichmentData? Data { get; set; }
    public string? Error { get; set; }
}

public class EnrichmentData
{
    public GeographicInfo? Geographic { get; set; }
    public EmailInsights? EmailInsights { get; set; }
    public PhoneInsights? PhoneInsights { get; set; }
    public CustomerProfile? CustomerProfile { get; set; }
    public string? LeadPriority { get; set; }
    public DateTime EnrichedAt { get; set; }
}

public class GeographicInfo
{
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? Population { get; set; }
    public string? MarketPotential { get; set; }
}

public class EmailInsights
{
    public string? CustomerType { get; set; }
    public string? TrustLevel { get; set; }
    public bool BusinessEmail { get; set; }
}

public class PhoneInsights
{
    public string? Carrier { get; set; }
    public string? Quality { get; set; }
    public bool Verified { get; set; }
}

public class CustomerProfile
{
    public bool LikelyFirstTimeBuyer { get; set; }
    public string? InterestLevel { get; set; }
    public string? RecommendedContactTime { get; set; }
}