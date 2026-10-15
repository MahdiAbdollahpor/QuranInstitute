namespace QuranInstitute.Application.Common.Interfaces;

public interface IDateTimeService
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
    string TodayShamsi();
    string ToShamsi(DateTime dt, bool withTime = false);
}