namespace NbpExchangeRatesWebApi.Dtos;

public class ExchangeRateDto
{
    public string CurrencyCode { get; set; }
    public string CurrencyName { get; set; }
    public decimal Rate { get; set; }
}