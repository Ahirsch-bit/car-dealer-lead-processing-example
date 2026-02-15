namespace CarDealer.LeadAutomation.Services.LeadEnrichment;

public class EnrichmentResponse
{
    public bool Enriched { get; set; }
    public EnrichmentResponseData? Data { get; set; }
    public string? Error { get; set; }
}

public class EnrichmentResponseData
{
    public GeographicData? Geographic { get; set; }
    public EmailInsightsData? EmailInsights { get; set; }
    public PhoneInsightsData? PhoneInsights { get; set; }
    public CustomerProfileData? CustomerProfile { get; set; }
    public string? LeadPriority { get; set; }
    public DateTime EnrichedAt { get; set; }
}

public class GeographicData
{
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? Population { get; set; }
    public string? MarketPotential { get; set; }
}

public class EmailInsightsData
{
    public string? CustomerType { get; set; }
    public string? TrustLevel { get; set; }
    public bool BusinessEmail { get; set; }
}

public class PhoneInsightsData
{
    public string? Carrier { get; set; }
    public string? Quality { get; set; }
    public bool Verified { get; set; }
}

public class CustomerProfileData
{
    public bool LikelyFirstTimeBuyer { get; set; }
    public string? InterestLevel { get; set; }
    public string? RecommendedContactTime { get; set; }
}

