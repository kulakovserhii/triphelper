namespace FlightService.Models
{
    public record FlightOffer(
        string ProviderOfferId,
        decimal Price, 
        string Currency,
        string PriceStatus,
        string CabinClass,
        bool RequiresSelfTransfer,
        BagAllowance? Bags, 
        FlightLeg Outbound);
}