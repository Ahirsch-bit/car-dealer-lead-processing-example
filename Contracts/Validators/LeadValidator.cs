using FluentValidation;

namespace CarDealer.LeadAutomation.Contracts.Validators;

public class LeadValidator:AbstractValidator<LeadRequest>
{
    private readonly IEmailValidator _emailValidator;
    
    public LeadValidator(IEmailValidator emailValidator)
    {
        _emailValidator = emailValidator;
        
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Email) || !string.IsNullOrWhiteSpace(x.Phone))
            .WithMessage("Either Email or Phone must be provided.");
        RuleFor(x=> x.Email)
            .MustAsync(async (email, token) =>await  _emailValidator.IsValidEmail(email))
            .WithMessage("Email is not valid. Must not be from a disposable email provider.");
    }
}