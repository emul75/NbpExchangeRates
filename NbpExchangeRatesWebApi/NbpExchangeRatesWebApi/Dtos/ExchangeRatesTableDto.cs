namespace NbpExchangeRatesWebApi.Dtos;

public class ExchangeRatesTableDto
{
    public IEnumerable<ExchangeRateDto> ExchangeRates { get; init; }
    public string NbpTableId { get; init; }
    public DateTime EffectiveDate { get; init; }
    public DateTime SavedAt { get; init; }
    public bool IsFromDatabase { get; init; }
    public string? Message { get; init; }
}