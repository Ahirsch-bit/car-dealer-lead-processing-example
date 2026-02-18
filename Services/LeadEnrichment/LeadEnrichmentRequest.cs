using System.Text.Json;
using CarDealer.LeadAutomation.Contracts;
using CarDealer.LeadAutomation.Repository;
using CarDealer.LeadAutomation.Services.Interfaces;

namespace CarDealer.LeadAutomation.Services.LeadEnrichment;

public class LeadEnrichmentRequest: ILeadEnrichmentRequest
{
    private readonly HttpClient _httpClient;
    private const string HOST = "http://mock-api:8001";
    private const string ENRICH_ENDPOINT = "/api/enrich";
    private const int TIMEOUT_MS = 5000; // 5 second timeout for each attempt
    private const int MAX_RETRIES = 3;
    private readonly ILogger<BranchRepository> _logger;

    public LeadEnrichmentRequest(ILogger<BranchRepository> logger)
    {
        _logger = logger;
        _httpClient = new HttpClient
        {
            Timeout =  TimeSpan.FromMilliseconds(TIMEOUT_MS)
        };
        
    }

    /// <summary>
    /// Sends an enrichment request to the API.
    /// Returns EnrichmentResponse if successful, null if request fails.
    /// </summary>
    public async Task<EnrichmentResponse?> EnrichLeadAsync(EnrichmentRequest? request)
    {
        if (request == null)
        {
            _logger.LogError("Enrichment request cannot be null. \n Request: {Request}", request);
            return null;
        }

        for (var attempt = 1; attempt <= MAX_RETRIES; attempt++)
        {
            var response = await TryEnrichAsync(request);

            if (response == null)
            {
                if (attempt < MAX_RETRIES)
                {
                    _logger.LogWarning("Enrichment request to {Host} timed out or returned no response. Retry {Attempt}/{MaxRetries}", HOST, attempt, MAX_RETRIES);
                    continue;
                }

                _logger.LogError("Enrichment request to {Host} failed with no response after {MaxRetries} attempts", HOST, MAX_RETRIES);
                return null;
            }

            if ((int)response.StatusCode >= 500 && (int)response.StatusCode <= 599)
            {
                if (attempt < MAX_RETRIES)
                {
                    _logger.LogWarning("Enrichment request to {Host} returned {StatusCode}. Retry {Attempt}/{MaxRetries}", HOST, response.StatusCode, attempt, MAX_RETRIES);
                    continue;
                }

                _logger.LogError("Enrichment request to {Host} failed with {StatusCode} after {MaxRetries} attempts", HOST, response.StatusCode, MAX_RETRIES);
                return null;
            }

            if (response.IsSuccessStatusCode)
            {
                return await DeserializeResponse(response);
            }

            // No retry for 4xx or other non-success responses.
            _logger.LogError("Enrichment request to {Host} failed. Status Code: {StatusCode}", HOST, response.StatusCode);
            return null;
        }

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
            
            return await _httpClient.PostAsync(url, content);
        }
        catch (System.Threading.Tasks.TaskCanceledException)
        {
            // Warning: Enrichment request timeout
            _logger.LogWarning("Enrichment request to {Host} timed out after {TimeoutMs}ms", HOST, TIMEOUT_MS);
            return null;
        }
        catch (HttpRequestException ex)
        {
            // Warning: Unable to reach enrichment API
            _logger.LogWarning("Unable to reach enrichment API at {Host}: {Message}", HOST, ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            // Error: Unexpected error during enrichment
            _logger.LogError("Unexpected error during enrichment: {Message}", ex.Message);
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
            // Error: Failed to deserialize enrichment response
            _logger.LogError("Failed to deserialize enrichment response: {Message}", ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            // Error: Unexpected error deserializing enrichment response
            _logger.LogError("Unexpected error deserializing enrichment response: {Message}", ex.Message);
            return null;
        }
    }
}