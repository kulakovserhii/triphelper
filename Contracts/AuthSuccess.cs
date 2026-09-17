namespace AuthService.Contracts
{
    public record AuthSuccess(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt): AuthResult;
}