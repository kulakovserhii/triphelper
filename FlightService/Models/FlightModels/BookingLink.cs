namespace FlightService.Models.FlightModels
{
    public record BookingLink(string Provider, string? ProviderType, string? FareName,
        string? Specificity, decimal? Price, string? Currency, string Url);
}
