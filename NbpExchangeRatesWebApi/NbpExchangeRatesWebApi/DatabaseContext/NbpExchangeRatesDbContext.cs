using Microsoft.EntityFrameworkCore;
using NbpExchangeRatesWebApi.Models;

namespace NbpExchangeRatesWebApi.DatabaseContext;

public class
    NbpExchangeRatesDbContext : DbContext
{
    public NbpExchangeRatesDbContext(DbContextOptions<NbpExchangeRatesDbContext> options) : base(options)
    {
    }

    public DbSet<ExchangeRate> ExchangeRates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new ExchangeRateConfiguration());
    }
}