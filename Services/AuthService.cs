using AuthService.Contracts;
using AuthService.Db;
using AuthService.Entities;
using AuthService.Repositories;

namespace AuthService.Services
{
    public class AuthService : IAuthService
    {
        private const string DummyHash = "$2a$12$CwTycUXWue0Thq9StjUM0uJ8/gwrhSk8yqHrR8YSjxq5eafvpDGwG";
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly AuthDbContext _context;
        private readonly IJwtProvider _jwtProvider;
        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, AuthDbContext context,
            IRefreshTokenRepository refreshTokenRepository, IJwtProvider jwtProvider)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _context = context;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtProvider = jwtProvider;
        }

        public async Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email, ct);
            if(user is null)
            {
                _passwordHasher.Verify(request.Password, DummyHash);
                return new AuthFailure("Wrong email or password");
            }
            if(!_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                return new AuthFailure("Wrong email or password");
                
            }
            return await IssueTokensAsync(user, request.DeviceInfo, ct);
        
        }

        public async Task<bool> LogoutAsync(string refreshToken, CancellationToken ct = default)
        {
            var hash = _jwtProvider.HashToken(refreshToken);
            return await _refreshTokenRepository.DeleteByTokenHashAsync(hash, ct);
        }

        public async Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
        {
            if (await _userRepository.EmailExistsAsync(request.Email))
                return new AuthFailure("User with this email already exists");
            var user = new User
            {
                Name = request.Name,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = _passwordHasher.Hash(request.Password),
                UserRoles = new List<UserRole> { new() { RoleId = 1 } }
            };
            await _userRepository.AddAsync(user, ct);
            await _context.SaveChangesAsync(ct);   
            user.UserRoles.First().Role = new Role { Id = 1, Name = "User" };
            return await IssueTokensAsync(user, request.DeviceInfo, ct);
        }
        private async Task<AuthResult> IssueTokensAsync(User user, string? deviceInfo, CancellationToken ct)
        {
            const int maxTokensPerUser = 3;
            await _refreshTokenRepository.EnforceTokenLimitAsync(user.Id, maxTokensPerUser, ct);
            var (acessToken, expiresAt) = _jwtProvider.GenerateAccessToken(user);
            var refreshTokenPlain = _jwtProvider.GenerateRefreshToken();
            var refreshToken = new RefreshToken
            {
                TokenHash = _jwtProvider.HashToken(refreshTokenPlain),
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                UserId = user.Id,
                DeviceInfo = deviceInfo
            };
            await _refreshTokenRepository.AddAsync(refreshToken, ct);
            await _context.SaveChangesAsync(ct);
            return new AuthSuccess(acessToken, refreshTokenPlain, expiresAt);
        }
    }
}
