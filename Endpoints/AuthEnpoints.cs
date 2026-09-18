using AuthService.Contracts;
using AuthService.Services;
using AuthService.Validators;

namespace AuthService.Endpoints
{
    public static class AuthEnpoints
    {
        public static void MapAuthEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/auth").WithTags("Auth");

            group.MapPost("/register", async (RegisterRequest request, IAuthService authService, CancellationToken ct) =>
            {
                var result = await authService.RegisterAsync(request, ct);
                return result switch
                {
                    AuthSuccess s => Results.Ok(new AuthResponse(s.AccessToken, s.RefreshToken, s.AccessTokenExpiresAt)),
                    AuthFailure f => Results.Conflict(new { error = f.Error }),
                    _ => Results.Problem()
                };
            }).AddEndpointFilter<ValidationFilter<RegisterRequest>>();

            group.MapPost("/login", async (LoginRequest request, IAuthService authService, CancellationToken ct) =>
            {
                var result = await authService.LoginAsync(request, ct);
                return result switch
                {
                    AuthSuccess s => Results.Ok(new AuthResponse(s.AccessToken, s.RefreshToken, s.AccessTokenExpiresAt)),
                    AuthFailure => Results.Unauthorized(),
                    _ => Results.Problem()
                };
            }).AddEndpointFilter<ValidationFilter<LoginRequest>>();

            group.MapPost("/logout", async (LogoutRequest request, IAuthService authService, CancellationToken ct) =>
            {
                var deleted = await authService.LogoutAsync(request.RefreshToken, ct);
                return deleted ? Results.NoContent() : Results.NotFound();
            });
        }
    }
}
