using AuthService.Db;
using AuthService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _context;
        public UserRepository(AuthDbContext context) => _context = context;
        public async Task<User> AddAsync(User user, CancellationToken ct = default)
        {
            _context.Users.Add(user);
            return user;
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
        {
            return await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Email == email, ct);
        }

        public async Task<User?> GetUserByEmailAsync(string email, CancellationToken ct = default)
        {
            return await _context.Users
                .AsNoTracking()
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .AsSplitQuery()
                .FirstOrDefaultAsync(u => u.Email == email, ct);
        }
    }
}
