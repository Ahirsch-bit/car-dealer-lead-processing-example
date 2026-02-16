using CarDealer.LeadAutomation.Contracts;

namespace CarDealer.LeadAutomation.Services.LeadEnrichment.Extensions;

public static class EnrichmentResponseExtensions
{
    private static int GetPriorityScore(this EnrichmentResponse enrichmentResponse)
    {
        var leadPriority = enrichmentResponse.Data.LeadPriority.ToLower();
        return leadPriority switch
        {
            "high" => 40,
            "medium" => 20,
            _ => 0
        };
    }

    private static int GetEmailTrustLevelScore(this EnrichmentResponse enrichmentResponse)
    {
        var trustLevel = enrichmentResponse.Data.EmailInsights.TrustLevel.ToString();
        return trustLevel == "high" ? 20 : 0;
    }

    private static int GetPhoneVerifiedScore(this EnrichmentResponse enrichmentResponse)
    {
        return enrichmentResponse.Data.PhoneInsights.Verified ? 20 : 0;
    }

    public static int GetEnrichmentScore(this EnrichmentResponse enrichmentResponse)
    {
        return GetPriorityScore(enrichmentResponse) +
               GetEmailTrustLevelScore(enrichmentResponse)+
               GetPhoneVerifiedScore(enrichmentResponse);
    }
}