namespace AuthService.Contracts
{
    public record AuthFailure(string Error) : AuthResult;
}
