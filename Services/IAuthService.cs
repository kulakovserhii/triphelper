using AuthService.Contracts;

namespace AuthService.Services
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
        Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken ct = default);
        Task<bool> LogoutAsync(string refreshToken, CancellationToken ct = default);
    }
}
