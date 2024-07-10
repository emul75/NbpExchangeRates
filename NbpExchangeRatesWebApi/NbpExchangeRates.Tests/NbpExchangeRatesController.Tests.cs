using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using FluentAssertions;
using NbpExchangeRatesWebApi.Controllers;
using NbpExchangeRatesWebApi.Dtos;
using NbpExchangeRatesWebApi.Interfaces;

public class CurrencyRatesControllerTests
{
    private readonly IExchangeRateService _service;
    private readonly CurrencyRatesController _controller;

    public CurrencyRatesControllerTests()
    {
        _service = Substitute.For<IExchangeRateService>();
        _controller = new CurrencyRatesController(_service);
    }

    [Fact]
    public async Task Get_ShouldReturnOkResult_WhenExchangeRatesAreAvailable()
    {
        // Arrange
        var expectedDto = new ExchangeRatesTableDto
        {
            NbpTableId = "132/A/NBP/2024",
            EffectiveDate = DateTime.UtcNow,
            SavedAt = DateTime.UtcNow,
            IsFromDatabase = false,
            ExchangeRates = new List<ExchangeRateDto>
            {
                new ExchangeRateDto
                {
                    CurrencyCode = "AUD",
                    CurrencyName = "dolar australijski",
                    Rate = 2.6536m
                }
            }
        };
        _service.GetAndSaveExchangeRates(Arg.Any<CancellationToken>()).Returns(expectedDto);

        // Act
        var result = await _controller.Get(CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(expectedDto);
    }

    [Fact]
    public async Task Get_ShouldReturnNotFoundResult_WhenExchangeRatesAreNotAvailable()
    {
        // Arrange
        _service.GetAndSaveExchangeRates(Arg.Any<CancellationToken>()).Returns((ExchangeRatesTableDto)null);

        // Act
        var result = await _controller.Get(CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }
}