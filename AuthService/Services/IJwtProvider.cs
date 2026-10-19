using AuthService.Entities;

namespace AuthService.Services
{
    public interface IJwtProvider
    {
        (string token, DateTime ExpiresAt) GenerateAccessToken(User user);
        string GenerateRefreshToken();
        string HashToken(string token);
    }
}
