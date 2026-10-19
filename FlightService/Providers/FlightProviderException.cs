using System.Net;
namespace FlightService.Providers
{
    public class FlightProviderException(string message, ProviderErrorKind kind, HttpStatusCode? statusCode = null): Exception(message)
    {
        public ProviderErrorKind Kind { get; } = kind;
        public HttpStatusCode? StatusCode { get; } = statusCode;
    }
}