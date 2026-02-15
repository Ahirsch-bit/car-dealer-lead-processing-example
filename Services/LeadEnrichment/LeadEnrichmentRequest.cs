using System.Text.Json;
using CarDealer.LeadAutomation.Contracts;

namespace CarDealer.LeadAutomation.Services.LeadEnrichment;

public class LeadEnrichmentRequest
{
    private readonly HttpClient _httpClient;
    private const string HOST = "http://localhost:8001";
    private const string ENRICH_ENDPOINT = "/api/enrich";
    private const int TIMEOUT_MS = 5000; // 5 second timeout for each attempt

    public LeadEnrichmentRequest()
    {
        _httpClient = new HttpClient();
    }

    /// <summary>
    /// Sends an enrichment request to the API.
    /// Returns EnrichmentResponse if successful, null if request fails.
    /// </summary>
    public async Task<EnrichmentResponse?> EnrichLeadAsync(EnrichmentRequest request)
    {
        if (request == null)
        {
            // TODO: Log error - Enrichment request cannot be null
            return null;
        }

        var response = await TryEnrichAsync(request);
        if (response?.IsSuccessStatusCode == true)
        {
            return await DeserializeResponse(response);
        }

        // TODO: Log error - Enrichment request to {HOST} failed
        return null;
    }

    /// <summary>
    /// Attempts to enrich a lead and returns the HTTP response
    /// </summary>
    private async Task<HttpResponseMessage?> TryEnrichAsync(EnrichmentRequest request)
    {
        try
        {
            var url = $"{HOST}{ENRICH_ENDPOINT}";
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            // Set timeout for this request
            using (var cts = new System.Threading.CancellationTokenSource(TIMEOUT_MS))
            {
                var response = await _httpClient.PostAsync(url, content, cts.Token);
                return response;
            }
        }
        catch (System.Threading.Tasks.TaskCanceledException)
        {
            // TODO: Log warning - Enrichment request to {HOST} timed out after {TIMEOUT_MS}ms
            return null;
        }
        catch (HttpRequestException ex)
        {
            // TODO: Log warning - Unable to reach enrichment API at {HOST}: {ex.Message}
            return null;
        }
        catch (Exception ex)
        {
            // TODO: Log error - Unexpected error during enrichment: {ex.Message}
            return null;
        }
    }

    /// <summary>
    /// Deserializes the HTTP response into an EnrichmentResponse object
    /// </summary>
    private async Task<EnrichmentResponse?> DeserializeResponse(HttpResponseMessage response)
    {
        try
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var enrichmentResponse = JsonSerializer.Deserialize<EnrichmentResponse>(responseBody,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return enrichmentResponse;
        }
        catch (JsonException ex)
        {
            // TODO: Log error - Failed to deserialize enrichment response: {ex.Message}
            return null;
        }
        catch (Exception ex)
        {
            // TODO: Log error - Unexpected error deserializing enrichment response: {ex.Message}
            return null;
        }
    }
}