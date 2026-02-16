using CarDealer.LeadAutomation.Repository.DTOs;

namespace CarDealer.LeadAutomation.Services.LeadEnrichment;

public static class CarModelExtensions
{
    private static int GetCategoryScore(this ModelDTO modelDto)
    {
        var model = modelDto.Category.ToLower();
        return model switch
        {
            "luxury" => 20,
            "electric" => 15,
            _ => 0
        };
    }

    private static int GetAvailabilityScore(this ModelDTO modelDto)
    {
        return modelDto.Availability.ToLower()== "in stock"? 10 : 0;
    }
    
    public static int GetCarScore(this ModelDTO modelDto)
    {
        return GetCategoryScore(modelDto) + GetAvailabilityScore(modelDto);
    }
}