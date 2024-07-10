using NbpExchangeRatesWebApi.Models;

namespace NbpExchangeRatesWebApi.Interfaces
{
    public interface IExchangeRateRepository
    {
        Task<IEnumerable<ExchangeRate>?> GetLatestExchangeRatesAsync(CancellationToken cancellationToken);
        Task<bool> AnyExchangeRatesAsync(CancellationToken cancellationToken);
        Task SaveExchangeRatesAsync(IEnumerable<ExchangeRate> exchangeRates, CancellationToken cancellationToken);
    }
}