namespace NbpExchangeRatesWebApi.Models;

public record ExchangeRate
{
    public Guid Id { get; init; }
    
    public required string CurrencyCode { get; init; }
    public required string CurrencyName { get; init; }
    public required string NbpTableId { get; init; }
    public required char NbpTableType { get; set; }
    public decimal Rate { get; init; }
    public DateTime EffectiveDate { get; init; }
    public DateTime SavedAt { get; init; } 
}