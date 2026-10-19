namespace FlightService.Providers
{
    public enum ProviderErrorKind
    {
        InvalidRequest,
        NotFound, 
        Unauthorized, 
        RateLimited, 
        Unavailable
    }
}
