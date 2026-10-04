namespace AuthService.Contracts
{
    public record GoogleLoginRequest(string IdToken, string? DeviceInfo);
}
