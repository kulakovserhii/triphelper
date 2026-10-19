
using FlightService.Models;
using FlightService.Models.FlightModels;
using FlightService.Providers;
using FlightService.Services;
using FlightService.Validators;
using System.Text.RegularExpressions;

namespace FlightService.Endpoints;

public static class FlightEndpoints
{
    private static readonly Regex OfferIdPattern = new("^[A-Za-z0-9]{8,64}$", RegexOptions.Compiled);

    public static void MapFlightEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/flights").WithTags("Flights");

        group.MapGet("/search", async (
            [AsParameters] SearchFlightsRequest request,
            IFlightSearchService service,
            CancellationToken ct) =>
        {
            var query = new FlightSearchQuery(
                request.Origin.ToUpperInvariant(), request.Destination.ToUpperInvariant(), request.Date);
            try
            {
                var offers = await service.SearchAsync(query, ct);
                return Results.Ok(new FlightSearchResponse(
                    query.Origin, query.Destination, query.DepartureDate, offers.Count, offers));
            }
            catch (FlightProviderException ex) { return ToProblem(ex); }
            catch (OperationCanceledException) when (!ct.IsCancellationRequested)
            {
                return Results.Problem("Провайдер не відповів вчасно", statusCode: StatusCodes.Status504GatewayTimeout);
            }
        })
        .AddEndpointFilter<ValidationFilter<SearchFlightsRequest>>()
        .RequireRateLimiting("search")
        .WithSummary("Пошук авіарейсів в один бік")
        .Produces<FlightSearchResponse>()
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status502BadGateway);

        group.MapGet("/{offerId}/booking-links", async (
            string offerId, IFlightSearchService service, CancellationToken ct) =>
        {
            if (!OfferIdPattern.IsMatch(offerId))
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["offerId"] = ["Некоректний ідентифікатор пропозиції"]
                });
            try
            {
                var options = await service.GetBookingOptionsAsync(offerId, ct);
                return Results.Ok(new BookingLinksResponse(offerId, options));
            }
            catch (FlightProviderException ex)
                when (ex.Kind is ProviderErrorKind.InvalidRequest or ProviderErrorKind.NotFound)
            {
                return Results.Problem("Пропозицію не знайдено або вона застаріла. Повторіть пошук",
                    statusCode: StatusCodes.Status404NotFound);
            }
            catch (FlightProviderException ex) { return ToProblem(ex); }
            catch (OperationCanceledException) when (!ct.IsCancellationRequested)
            {
                return Results.Problem("Провайдер не відповів вчасно", statusCode: StatusCodes.Status504GatewayTimeout);
            }
        })
        .RequireRateLimiting("search")
        .WithSummary("Посилання на купівлю для обраної пропозиції")
        .Produces<BookingLinksResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static IResult ToProblem(FlightProviderException ex) => ex.Kind switch
    {
        ProviderErrorKind.InvalidRequest => Results.Problem("Перевірте міста та дату пошуку", statusCode: 400),
        ProviderErrorKind.RateLimited => Results.Problem("Забагато запитів до провайдера, спробуйте за хвилину", statusCode: 503),
        _ => Results.Problem("Сервіс пошуку рейсів тимчасово недоступний", statusCode: 502)
    };
}