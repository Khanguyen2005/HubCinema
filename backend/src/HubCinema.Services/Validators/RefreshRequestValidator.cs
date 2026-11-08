using FluentValidation;
using HubCinema.Services.Dtos.Auth;

namespace HubCinema.Services.Validators;

public class RefreshRequestValidator : AbstractValidator<RefreshRequest>
{
    public RefreshRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}
