using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NbpExchangeRatesWebApi.Interfaces;
using NbpExchangeRatesWebApi.Models;
using NbpExchangeRatesWebApi.Services;
using NSubstitute;

public class ExchangeRateServiceTests
{
    private readonly IExchangeRateRepository _repository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ExchangeRateService> _logger;
    private readonly ExchangeRateService _service;

    public ExchangeRateServiceTests()
    {
        _repository = Substitute.For<IExchangeRateRepository>();
        _configuration = Substitute.For<IConfiguration>();
        _logger = Substitute.For<ILogger<ExchangeRateService>>();
        _service = new ExchangeRateService(new HttpClient(), _repository, _configuration, _logger);
    }

    [Fact]
    public async Task GetAndSaveExchangeRates_ShouldReturnExchangeRatesFromApi_WhenApiIsAvailable()
    {
        // Arrange
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(new[]
            {
                new {
                    table = "A",
                    no = "133/A/NBP/2024",
                    effectiveDate = "2024-07-10",
                    rates = new[]
                    {
                        new { currency = "bat (Tajlandia)", code = "THB", mid = 0.1081 },
                        new { currency = "dolar amerykański", code = "USD", mid = 3.9324 },
                        new { currency = "dolar australijski", code = "AUD", mid = 2.6504 },
                        new { currency = "dolar Hongkongu", code = "HKD", mid = 0.5034 }
                    }
                }
            }))
        };

        var handler = new FakeHttpMessageHandler(response);
        var client = new HttpClient(handler);

        var service = new ExchangeRateService(client, _repository, _configuration, _logger);

        // Act
        var result = await service.GetAndSaveExchangeRates(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ExchangeRates.Should().HaveCount(4);
        result.ExchangeRates.First().CurrencyCode.Should().Be("THB");
        result.ExchangeRates.First().Rate.Should().Be(0.1081m);
    }

    [Fact]
    public async Task GetAndSaveExchangeRates_ShouldReturnExchangeRatesFromDatabase_WhenApiIsUnavailable()
    {
        // Arrange
        var expectedExchangeRates = new List<ExchangeRate>
        {
            new ExchangeRate
            {
                Id = Guid.NewGuid(),
                CurrencyCode = "USD",
                CurrencyName = "dolar amerykański",
                NbpTableId = "133/A/NBP/2024",
                NbpTableType = 'A',
                Rate = 3.9324m,
                EffectiveDate = new DateTime(2024, 7, 10),
                SavedAt = DateTime.UtcNow
            }
        };

        _repository.GetLatestExchangeRatesAsync(Arg.Any<CancellationToken>()).Returns(expectedExchangeRates);
        _repository.AnyExchangeRatesAsync(Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _service.GetAndSaveExchangeRates(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ExchangeRates.Should().HaveCount(1);
        result.ExchangeRates.First().CurrencyCode.Should().Be("USD");
        result.ExchangeRates.First().Rate.Should().Be(3.9324m);
        result.IsFromDatabase.Should().BeTrue();
    }

    private class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;

        public FakeHttpMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_response);
        }
    }
}
