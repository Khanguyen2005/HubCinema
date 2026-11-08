using HubCinema.API.Extensions;
using HubCinema.API.Filters;
using HubCinema.API.Middlewares;
using HubCinema.API.Security;
using HubCinema.Common.Responses;
using HubCinema.Repositories;
using HubCinema.Services;
using HubCinema.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllers(options => options.Filters.Add<ValidationActionFilter>())
    // Don't surface System.Text.Json's raw exception text — it leaks DTO type names and paths.
    .AddJsonOptions(options => options.AllowInputFormatterExceptionMessages = false)
    .ConfigureApiBehaviorOptions(options =>
    {
        // [ApiController]'s automatic model-state check (malformed JSON, bad types, missing
        // required fields) runs before our ValidationActionFilter. Map its 400 to the same
        // ApiResponse envelope so every validation failure has one consistent shape.
        options.InvalidModelStateResponseFactory = context =>
        {
            // ModelState keys starting with "$" come from System.Text.Json failing to parse or
            // convert the body. Return one friendly message instead of the internal detail.
            if (context.ModelState.Keys.Any(key => key.StartsWith('$')))
            {
                return new BadRequestObjectResult(ApiResponse.Fail("Request body is invalid or malformed."));
            }

            var errors = context.ModelState
                .SelectMany(entry => entry.Value!.Errors.Select(e => e.ErrorMessage))
                .Where(message => !string.IsNullOrWhiteSpace(message))
                .ToList();

            return new BadRequestObjectResult(ApiResponse.Fail("Validation failed.", errors));
        };
    });
builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Global error handling first so it wraps the whole pipeline below.
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
