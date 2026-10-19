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

        public async Task EnforceTokenLimitAsync(int userId, int maxTokens, CancellationToken ct = default)
        {
            var idsToDelete = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .OrderByDescending(rt => rt.CreatedAt)
                .Skip(maxTokens - 1)
                .Select(rt => rt.Id)
                .ToListAsync(ct);
            if (idsToDelete.Count == 0) return;
            await _context.RefreshTokens.Where(rt => idsToDelete.Contains(rt.Id)).ExecuteDeleteAsync(ct);
        }

        public async Task<RefreshToken?> GetByTokenHashAsync(string hashToken, CancellationToken ct = default)
        {
            return await _context.RefreshTokens.AsNoTracking()
                .FirstOrDefaultAsync(rt => rt.TokenHash == hashToken, ct);
        }
    }
}
