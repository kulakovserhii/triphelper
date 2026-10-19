using AuthService.Entities;

namespace AuthService.Repositories
{
    public interface IUserRepository
    {
        Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
        Task<User?> GetUserByEmailAsync(string email, CancellationToken ct = default);
        Task<User> AddAsync(User user, CancellationToken ct = default);
        Task<User>? GetByGoogleIdAsync(string googleId, CancellationToken ct = default);
    }
}
