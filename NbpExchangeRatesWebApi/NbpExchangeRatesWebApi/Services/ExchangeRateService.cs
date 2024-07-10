using System.Text.Json;
using NbpExchangeRatesWebApi.Dtos;
using NbpExchangeRatesWebApi.Interfaces;
using NbpExchangeRatesWebApi.Models;

namespace NbpExchangeRatesWebApi.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly HttpClient _httpClient;
        private readonly IExchangeRateRepository _repository;
        private readonly ILogger<ExchangeRateService> _logger;
        private readonly string _nbpApiUrl;

        public ExchangeRateService(HttpClient httpClient, IExchangeRateRepository repository, IConfiguration configuration,
            ILogger<ExchangeRateService> logger)
        {
            _httpClient = httpClient;
            _repository = repository;
            _logger = logger;
            _nbpApiUrl = configuration["NbpApiUrl:TableA"] ?? throw new ArgumentNullException("NbpApiUrl:TableA");
        }

        public async Task<ExchangeRatesTableDto?> GetAndSaveExchangeRates(CancellationToken cancellationToken)
        {
            var isFromDatabase = false;
            string? errorMessage = null;

            var exchangeRates = await GetExchangeRatesFromNbpApi(cancellationToken);
            if (exchangeRates == null)
            {
                exchangeRates = await _repository.GetLatestExchangeRatesAsync(cancellationToken);
                isFromDatabase = true;

                errorMessage = "An error occurred while downloading and parsing data from NBP API. " +
                               "Exchange rates data taken from the database instead.";
                _logger.LogError(errorMessage);
            }

            if (exchangeRates == null)
            {
                errorMessage = "An error occurred while downloading and parsing data from NBP API. " +
                               "No exchange rates available in the database.";
                _logger.LogError(errorMessage);

                return null;
            }

            await _repository.SaveExchangeRatesAsync(exchangeRates, cancellationToken);

            return MapToDto(exchangeRates, isFromDatabase, errorMessage);
        }

        private async Task<IEnumerable<ExchangeRate>?> GetExchangeRatesFromNbpApi(CancellationToken cancellationToken)
        {
            try
            {
                var response = await _httpClient.GetStringAsync(_nbpApiUrl, cancellationToken);
                using var jsonDocument = JsonDocument.Parse(response);
                var exchangeRates = MapJsonToModel(jsonDocument);
                return exchangeRates;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Request error while accessing NBP API.");
            }
            catch (JsonException e)
            {
                _logger.LogError(e, "JSON parsing error while processing NBP API response.");
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, "Invalid operation error while processing NBP API response.");
            }

            return null;
        }

        private static IEnumerable<ExchangeRate> MapJsonToModel(JsonDocument jsonDocument)
        {
            var exchangeRatesTable = jsonDocument.RootElement[0];

            var effectiveDate = exchangeRatesTable.GetProperty("effectiveDate").GetDateTime();
            var nbpTableNumber = exchangeRatesTable.GetProperty("no").GetString();
            var tableType = exchangeRatesTable.GetProperty("table").GetString();
            var ratesArray = exchangeRatesTable.GetProperty("rates").EnumerateArray().ToList();  // Kopiowanie danych do listy

            if (string.IsNullOrEmpty(tableType) || tableType.Length != 1)
                throw new InvalidOperationException("Invalid table code received from NBP API");

            return ratesArray.Select(exchangeRateElement => new ExchangeRate
            {
                Id = Guid.NewGuid(),
                CurrencyCode = exchangeRateElement.GetProperty("code").GetString()
                       ?? throw new InvalidOperationException("Code cannot be null"),
                CurrencyName = exchangeRateElement.GetProperty("currency").GetString()
                       ?? throw new InvalidOperationException("Currency cannot be null"),
                NbpTableId = nbpTableNumber
                             ?? throw new InvalidOperationException("Table number cannot be null"),
                NbpTableType = tableType[0],
                Rate = exchangeRateElement.GetProperty("mid").GetDecimal(),
                EffectiveDate = effectiveDate,
                SavedAt = DateTime.UtcNow
            }).ToList();
        }

        private static ExchangeRatesTableDto MapToDto(IEnumerable<ExchangeRate> exchangeRates, bool isFromDatabase = false,
            string? message = null)
        {
            var exchangeRateDtos = exchangeRates.Select(rate => new ExchangeRateDto
            {
                CurrencyCode = rate.CurrencyCode,
                CurrencyName = rate.CurrencyName,
                Rate = rate.Rate
            }).ToList();

            if (exchangeRateDtos.Count == 0)
            {
                return new ExchangeRatesTableDto
                {
                    ExchangeRates = exchangeRateDtos,
                    IsFromDatabase = isFromDatabase,
                    Message = message
                };
            }

            var er = exchangeRates.First();
            return new ExchangeRatesTableDto
            {
                ExchangeRates = exchangeRateDtos,
                NbpTableId = er.NbpTableId,
                EffectiveDate = er.EffectiveDate,
                SavedAt = er.SavedAt,
                IsFromDatabase = isFromDatabase,
                Message = message
            };
        }

    }
}
