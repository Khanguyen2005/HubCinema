namespace HubCinema.Services.Dtos.Auth;

public record RegisterRequest(
    string FullName,
    string Email,
    string PhoneNumber,
    string Password,
    DateOnly? DateOfBirth);
