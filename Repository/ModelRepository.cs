using CarDealer.LeadAutomation.Repository.DTOs;
using CarDealer.LeadAutomation.Repository.Interfaces;

namespace CarDealer.LeadAutomation.Repository;

public class ModelRepository:IModelRepository
{
    private readonly string _modelsFilePath;
    private List<ModelDTO>? _modelsCache;
    private readonly ILogger<ModelRepository> _logger;

    public ModelRepository(ILogger<ModelRepository> logger, string? modelsFilePath = null)
    {
        _logger = logger;
        // Default path relative to the application root
        _modelsFilePath = modelsFilePath ?? Path.Combine(AppContext.BaseDirectory, "Data", "car_models.txt");
    }

    /// <summary>
    /// Retrieves a car model by its Model ID with caching logic.
    /// First checks cache, if not found reloads from file, if still not found returns null and logs error.
    /// </summary>
    public ModelDTO? GetModelById(string modelId)
    {
        if (string.IsNullOrWhiteSpace(modelId))
        {
            // Error: Model ID cannot be null or empty
            _logger.LogError("Model ID cannot be null or empty");
            return null;
        }

        // First attempt: search in cache
        if (_modelsCache != null)
        {
            var cachedModel = _modelsCache.FirstOrDefault(m => m.ModelID == modelId);
            if (cachedModel != null)
                return cachedModel;
        }

        // Second attempt: reload from file and search
        _modelsCache = ParseModelsFile();
        var model = _modelsCache.FirstOrDefault(m => m.ModelID == modelId);
        
        if (model == null)
        {
            // Error: Model not found
            _logger.LogError("Model with ID {ModelId} not found in car_models.txt", modelId);
        }
        
        return model;
    }

    /// <summary>
    /// Parses the car_models.txt file and returns a list of all models
    /// </summary>
    private List<ModelDTO> ParseModelsFile()
    {
        var models = new List<ModelDTO>();

        try
        {
            if (!File.Exists(_modelsFilePath))
            {
                // Error: Car models file not found
                _logger.LogError("Car models file not found at {FilePath}", _modelsFilePath);
                return models;
            }

            var lines = File.ReadAllLines(_modelsFilePath);
            ModelDTO? currentModel = null;

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                // Skip empty lines and separator lines
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("================") || line.StartsWith("----------------"))
                    continue;

                // Check for Model ID line
                if (line.StartsWith("Model ID:"))
                {
                    // Save previous model if exists
                    if (currentModel != null)
                        models.Add(currentModel);

                    currentModel = new ModelDTO
                    {
                        ModelID = line.Replace("Model ID:", string.Empty).Trim()
                    };
                }
                else if (line.StartsWith("Brand:") && currentModel != null)
                {
                    currentModel.Brand = line.Replace("Brand:", string.Empty).Trim();
                }
                else if (line.StartsWith("Model:") && currentModel != null)
                {
                    currentModel.Model = line.Replace("Model:", string.Empty).Trim();
                }
                else if (line.StartsWith("Year:") && currentModel != null)
                {
                    currentModel.Year = line.Replace("Year:", string.Empty).Trim();
                }
                else if (line.StartsWith("Category:") && currentModel != null)
                {
                    currentModel.Category = line.Replace("Category:", string.Empty).Trim();
                }
                else if (line.StartsWith("Engine:") && currentModel != null)
                {
                    currentModel.Engine = line.Replace("Engine:", string.Empty).Trim();
                }
                else if (line.StartsWith("Price Range:") && currentModel != null)
                {
                    currentModel.PriceRange = line.Replace("Price Range:", string.Empty).Trim();
                }
                else if (line.StartsWith("Fuel Economy:") && currentModel != null)
                {
                    currentModel.FuelEconomy = line.Replace("Fuel Economy:", string.Empty).Trim();
                }
                else if (line.StartsWith("Availability:") && currentModel != null)
                {
                    currentModel.Availability = line.Replace("Availability:", string.Empty).Trim();
                }
                else if (line.StartsWith("Lead Time:") && currentModel != null)
                {
                    currentModel.LeadTime = line.Replace("Lead Time:", string.Empty).Trim();
                }
                else if (line.StartsWith("Warranty:") && currentModel != null)
                {
                    currentModel.Warranty = line.Replace("Warranty:", string.Empty).Trim();
                }
                else if (line.StartsWith("Popular:") && currentModel != null)
                {
                    currentModel.Popular = line.Replace("Popular:", string.Empty).Trim();
                }
                else if (line.StartsWith("Features:") && currentModel != null)
                {
                    // Features section starts, read feature lines until next key or empty section
                    i++;
                    while (i < lines.Length)
                    {
                        var featureLine = lines[i];

                        // Stop if we hit a new section or key-value pair
                        if (string.IsNullOrWhiteSpace(featureLine) || 
                            (!featureLine.StartsWith("  -") && featureLine.Contains(":")))
                            break;

                        // Add feature if it starts with "  -"
                        if (featureLine.StartsWith("  -"))
                        {
                            var feature = featureLine.Replace("  -", string.Empty).Trim();
                            if (!string.IsNullOrWhiteSpace(feature))
                                currentModel.Features.Add(feature);
                        }

                        i++;
                    }
                    i--; // Adjust for loop increment
                }
            }

            // Add the last model if exists
            if (currentModel != null)
                models.Add(currentModel);
        }
        catch (Exception ex)
        {
            // Error: Exception reading car models file
            _logger.LogError("Exception reading car models file: {Message}", ex.Message);
        }

        return models;
    }
}