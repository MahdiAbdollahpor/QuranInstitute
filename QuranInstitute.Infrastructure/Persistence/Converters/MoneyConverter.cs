using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuranInstitute.Domain.ValueObjects;

namespace QuranInstitute.Infrastructure.Persistence.Converters;

public class MoneyConverter : ValueConverter<Money, decimal>
{
    public MoneyConverter()
        : base(
            money => money.Amount,
            value => Money.FromRial(value))
    {
    }
}