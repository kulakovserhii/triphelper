using AuthService.Db;
using AuthService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AuthDbContext _context;
        public RefreshTokenRepository(AuthDbContext context) => _context = context;
        public async Task AddAsync(RefreshToken token, CancellationToken ct = default)
        {
            await _context.RefreshTokens.AddAsync(token, ct);
        }

        public async Task<bool> DeleteByTokenHashAsync(string tokenHash, CancellationToken ct = default)
        {
            var rowsAffected = await _context.RefreshTokens.Where(rt => rt.TokenHash == tokenHash)
               .ExecuteDeleteAsync(ct);
            return rowsAffected > 0;
        }

        public async Task<RefreshToken?> GetByTokenHashAsync(string hashToken, CancellationToken ct = default)
        {
            return await _context.RefreshTokens.AsNoTracking()
                .FirstOrDefaultAsync(rt => rt.TokenHash == hashToken, ct);
        }
    }
}
