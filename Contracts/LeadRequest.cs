using System.ComponentModel.DataAnnotations;

namespace CarDealer.LeadAutomation.Contracts;

public class LeadRequest
{
    [Required(ErrorMessage  = "BranchID is required.")]
    [RegularExpression(@"^\d+$", ErrorMessage = "BranchID must be numeric only.")]
    public string BranchID { get; set; }
    public string WorkerCode { get; set; }
    public string AskedCar { get; set; }
    [Required(ErrorMessage = "FirstName is required.")]
    public string FirstName { get; set; }
    [Required(ErrorMessage = "LastName is required.")]
    public string LastName { get; set; }
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string Email { get; set; }
    public string Phone { get; set; }
    public string FromWebSite { get; set; }
    public string Area { get; set; }
}