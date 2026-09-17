using AuthService.Entities;

namespace AuthService.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token, CancellationToken ct = default);
        Task<RefreshToken?> GetByTokenHashAsync(string hashToken, CancellationToken ct = default);
        Task<bool> DeleteByTokenHashAsync(string tokenHash, CancellationToken ct = default);
    }
}
