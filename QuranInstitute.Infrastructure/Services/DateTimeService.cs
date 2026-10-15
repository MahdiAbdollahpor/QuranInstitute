using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Shared.Helpers;

namespace QuranInstitute.Infrastructure.Services;

public class DateTimeService : IDateTimeService
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
    public string TodayShamsi() => MyDateTime.TodayShamsi();
    public string ToShamsi(DateTime dt, bool withTime = false)
        => dt.GetShamsiDateFromGregorian(withTime);
}