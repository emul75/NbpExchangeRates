using Microsoft.EntityFrameworkCore;
using NbpExchangeRatesWebApi.DatabaseContext;
using NbpExchangeRatesWebApi.Interfaces;
using NbpExchangeRatesWebApi.Models;

namespace NbpExchangeRatesWebApi.Repositories
{
    public class ExchangeRateRepository : IExchangeRateRepository
    {
        private readonly NbpExchangeRatesDbContext _dbContext;

        public ExchangeRateRepository(NbpExchangeRatesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ExchangeRate>?> GetLatestExchangeRatesAsync(CancellationToken cancellationToken)
        {
            if (!await _dbContext.ExchangeRates.AnyAsync(cancellationToken))
                return null;

            var latestDate = _dbContext.ExchangeRates.Max(er => er.EffectiveDate);
            return await _dbContext.ExchangeRates
                .Where(er => er.EffectiveDate == latestDate)
                .ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<bool> AnyExchangeRatesAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.ExchangeRates.AnyAsync(cancellationToken);
        }

        public async Task SaveExchangeRatesAsync(IEnumerable<ExchangeRate> exchangeRates, CancellationToken cancellationToken)
        {
            await _dbContext.ExchangeRates.AddRangeAsync(exchangeRates, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}