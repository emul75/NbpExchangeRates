using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NbpExchangeRatesWebApi.Models;

namespace NbpExchangeRatesWebApi.DatabaseContext;

public class ExchangeRateConfiguration : IEntityTypeConfiguration<ExchangeRate>
{
    public void Configure(EntityTypeBuilder<ExchangeRate> builder)
    {
        builder.ToTable("ExchangeRate");
        builder.HasKey(er => er.Id);
        builder.Property(er => er.CurrencyCode)
            .IsRequired()
            .HasMaxLength(3);
        builder.Property(er => er.CurrencyName)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(er => er.NbpTableId)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(er => er.NbpTableType)
            .IsRequired()
            .HasMaxLength(1)
            .HasColumnType("char(1)");
        builder.Property(er => er.Rate) 
            .IsRequired()
            .HasColumnType("decimal")
            .HasPrecision(18,15);
        builder.Property(er => er.EffectiveDate)
            .IsRequired();
        builder.Property(er => er.SavedAt)
            .IsRequired();
    }
}