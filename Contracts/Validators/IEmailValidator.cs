namespace CarDealer.LeadAutomation.Contracts.Validators;

public interface IEmailValidator
{
    Task<bool> IsValidEmail(string email);
}