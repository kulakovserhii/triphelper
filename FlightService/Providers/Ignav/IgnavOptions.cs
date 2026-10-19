namespace FlightService.Providers.Ignav
{
    public class IgnavOptions
    {
        public string BaseUrl { get; set; } = "https://ignav.com";
        public string ApiKey { get; set; } = string.Empty;
    }
}