namespace AuthService.Contracts
{
    public record LoginRequest(string Email, string Password, string? DeviceInfo);
}
