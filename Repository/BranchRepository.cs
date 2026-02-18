using CarDealer.LeadAutomation.Repository.DTOs;
using CarDealer.LeadAutomation.Repository.Interfaces;
using OfficeOpenXml;

namespace CarDealer.LeadAutomation.Repository;

public class BranchRepository:IBranchRepository
{
    private const int DEFAULT_BRANCH_ID = 400;
    private readonly string _excelPath;
    private List<BranchDTO>? _branchesCache;
    private readonly ILogger<BranchRepository> _logger;

    public BranchRepository(ILogger<BranchRepository> logger, string? excelPath = null)
    {
        _logger = logger;
        // Default path relative to the application root
        _excelPath = excelPath ?? Path.Combine(AppContext.BaseDirectory, "Data", "branch_config.xlsx");
    }

    /// <summary>
    /// Reads all branches from the Excel file and returns a list
    /// </summary>
    private List<BranchDTO> ReadExcelFile()
    {
        var branches = new List<BranchDTO>();

        try
        {
            if (!File.Exists(_excelPath))
            {
                _logger.LogWarning("Branch configuration Excel file not found at {ExcelPath}", _excelPath);
                return branches;
            }

            using (var package = new ExcelPackage(new FileInfo(_excelPath)))
            {
                var worksheet = package.Workbook.Worksheets[0];

                if (worksheet.Dimension == null || worksheet.Dimension.Rows < 2)
                    return branches;

                for (var row = 2; row <= worksheet.Dimension.Rows; row++)
                {
                    var branch = new BranchDTO
                    {
                        BranchID = int.TryParse(worksheet.Cells[row, 1].Value?.ToString(), out var branchId) ? branchId : 0,
                        Name = worksheet.Cells[row, 2].Value?.ToString(),
                        Region = worksheet.Cells[row, 3].Value?.ToString(),
                        City = worksheet.Cells[row, 4].Value?.ToString(),
                        Address = worksheet.Cells[row, 5].Value?.ToString(),
                        Manager = worksheet.Cells[row, 6].Value?.ToString(),
                        Email = worksheet.Cells[row, 7].Value?.ToString(),
                        Phone = worksheet.Cells[row, 8].Value?.ToString(),
                        WorkingHours = worksheet.Cells[row, 9].Value?.ToString(),
                        Specialties = worksheet.Cells[row, 10].Value?.ToString(),
                        Languages = worksheet.Cells[row, 11].Value?.ToString()
                    };

                    if (branch.BranchID > 0)
                        branches.Add(branch);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception reading Excel file: {ex.Message} \n {ex.StackTrace}", ex.Message);
        }

        return branches;
    }

    /// <summary>
    /// Retrieves a branch by its BranchID with smart caching logic.
    /// First checks cache, if not found refreshes from Excel, if still not found returns default branch (400).
    /// </summary>
    public BranchDTO GetBranchById(int branchId)
    {
        // First attempt: search in cache
        var cachedBranch = _branchesCache?.FirstOrDefault(b => b.BranchID == branchId);
        if (cachedBranch != null)
            return cachedBranch;

        // Second attempt: reload from Excel and search
        _branchesCache = ReadExcelFile();
        var branch = _branchesCache.FirstOrDefault(b => b.BranchID == branchId)??
                     GetDefaultBranch(); 
        _logger.LogWarning(
                   $"Branch with ID {branchId} not found in Excel file. Returning default branch {DEFAULT_BRANCH_ID}");
               GetDefaultBranch();
        return branch;
    }

    /// <summary>
    /// Returns the default branch (Branch ID 400)
    /// </summary>
    private BranchDTO GetDefaultBranch()
    {
        // First try to get from cache
        var cachedDefault = _branchesCache?.FirstOrDefault(b => b.BranchID == DEFAULT_BRANCH_ID);
        if (cachedDefault != null)
            return cachedDefault;

        // If not in cache, try to read from Excel
        _branchesCache ??= ReadExcelFile();
        var excelDefault = _branchesCache.FirstOrDefault(b => b.BranchID == DEFAULT_BRANCH_ID);
        if (excelDefault != null)
            return excelDefault;

        // Fallback: return hardcoded default branch (ID 400)
        return new BranchDTO
        {
            BranchID = 400,
            Name = "Tel Aviv Showroom",
            Region = "Center",
            City = "Tel Aviv",
            Address = "Menachem Begin Rd 132, Tel Aviv",
            Manager = "David Cohen",
            Email = "telaviv@dealership.co.il",
            Phone = "03-5551234",
            WorkingHours = "Sun-Thu 9:00-19:00, Fri 9:00-14:00",
            Specialties = "Luxury, Electric, SUV",
            Languages = "Hebrew, English, Russian"
        };
    }
}