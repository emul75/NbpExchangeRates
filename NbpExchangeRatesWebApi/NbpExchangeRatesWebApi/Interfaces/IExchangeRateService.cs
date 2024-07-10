using NbpExchangeRatesWebApi.Dtos;

namespace NbpExchangeRatesWebApi.Interfaces;

public interface IExchangeRateService
{
    Task<ExchangeRatesTableDto?> GetAndSaveExchangeRates(CancellationToken cancellationToken);
}