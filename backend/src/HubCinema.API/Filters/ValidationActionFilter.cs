using FluentValidation;
using HubCinema.Common.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HubCinema.API.Filters;

/// <summary>
/// Runs any registered FluentValidation <see cref="IValidator{T}"/> against each action
/// argument before the action executes. On failure it throws <see cref="BadRequestException"/>,
/// which the global exception middleware turns into a 400 <c>ApiResponse</c> with the error list.
/// </summary>
public class ValidationActionFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _services;

    public ValidationActionFilter(IServiceProvider services) => _services = services;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
            {
                continue;
            }

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            if (_services.GetService(validatorType) is not IValidator validator)
            {
                continue;
            }

            var result = await validator.ValidateAsync(new ValidationContext<object>(argument));
            if (!result.IsValid)
            {
                var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
                throw new BadRequestException("Validation failed.", errors);
            }
        }

        await next();
    }
}
