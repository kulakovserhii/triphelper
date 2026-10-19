using FlightService.Services;
using FluentValidation;

namespace FlightService.Validators
{
    public class SearchFlightsRequestValidator: AbstractValidator<SearchFlightsRequest>
    {
        public SearchFlightsRequestValidator(IAirportCatalog airports)
        {
            RuleFor(x => x.Origin).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Вкажіть аеропорт вильоту")
                .Matches("^[A-Za-z]{3}$").WithMessage("Код аеропорту має містити рівно 3 літери, наприклад CDG")
                .Must(c => airports.GetByIata(c) is not null)
                .WithMessage("Невідомий код аеропорту «{PropertyValue}». Оберіть аеропорт із підказок");

            RuleFor(x => x.Destination).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Вкажіть аеропорт прильоту")
                .Matches("^[A-Za-z]{3}$").WithMessage("Код аеропорту має містити рівно 3 літери, наприклад LAX")
                .Must(c => airports.GetByIata(c) is not null)
                .WithMessage("Невідомий код аеропорту «{PropertyValue}». Оберіть аеропорт із підказок");

            RuleFor(x => x)
                .Must(x => !string.Equals(x.Origin, x.Destination, StringComparison.OrdinalIgnoreCase))
                .WithMessage("Аеропорти вильоту і прильоту мають бути різними");

            RuleFor(x => x.Date)
                .Must(d => d >= DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Дата вильоту не може бути в минулому");
        }
    }
}
