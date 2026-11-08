using System.Text;
using FluentValidation;
using HubCinema.Services.Dtos.Auth;

namespace HubCinema.Services.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^0\d{9}$")
            .WithMessage("Phone number must be 10 digits starting with 0.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            // BCrypt only uses the first 72 bytes of a password; cap it so no accepted password
            // has a silently-ignored suffix (non-ASCII chars reach the limit in fewer characters).
            .Must(password => password is not null && Encoding.UTF8.GetByteCount(password) <= 72)
            .WithMessage("Password must be at most 72 bytes.");

        RuleFor(x => x.DateOfBirth!.Value)
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Date of birth must be in the past.")
            .When(x => x.DateOfBirth is not null);
    }
}
