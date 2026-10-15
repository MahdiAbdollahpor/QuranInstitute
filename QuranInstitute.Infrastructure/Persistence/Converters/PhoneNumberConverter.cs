using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuranInstitute.Domain.ValueObjects;

namespace QuranInstitute.Infrastructure.Persistence.Converters;

public class PhoneNumberConverter : ValueConverter<PhoneNumber, string>
{
    public PhoneNumberConverter()
        : base(
            phone => phone.Value,
            value => PhoneNumber.Create(value))
    {
    }
}