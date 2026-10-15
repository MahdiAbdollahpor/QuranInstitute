using System.Globalization;
using QuranInstitute.Shared.Helpers;

namespace QuranInstitute.Shared.Helpers;

public static class MyDateTime
{
    private static readonly PersianCalendar _pc = new();

    // ─── Gregorian → Shamsi ────────────────────────────
    public static string GetShamsiDateFromGregorian(this DateTime value, bool time = true)
    {
        try
        {
            var year = _pc.GetYear(value);
            var month = _pc.GetMonth(value);
            var day = _pc.GetDayOfMonth(value);

            if (!time)
                return $"{year}/{month:00}/{day:00}";

            var hour = _pc.GetHour(value);
            var minute = _pc.GetMinute(value);
            return $"{year}/{month:00}/{day:00}-{hour:00}:{minute:00}";
        }
        catch
        {
            return "متاسفانه تاریخ قابل بازیابی نبود";
        }
    }

    public static int GetYearOfGregorianDateInShamsi(DateTime date) => _pc.GetYear(date);
    public static int GetMonthOfGregorianDateInShamsi(DateTime date) => _pc.GetMonth(date);
    public static int GetDayOfMonthOfGregorianDateInShamsi(DateTime date) => _pc.GetDayOfMonth(date);

    public static int GetYearOfGregorianDateInShamsiEx(this DateTime? value)
        => value.HasValue ? GetYearOfGregorianDateInShamsi(value.Value) : 0;

    // ★ Bug اصلاح شد: قبلاً Day را برمیگرداند
    public static int GetMonthOfGregorianDateInShamsiEx(this DateTime? value)
        => value.HasValue ? GetMonthOfGregorianDateInShamsi(value.Value) : 0;

    // ─── Shamsi → Gregorian ────────────────────────────
    public static DateTime GetGregorianDateFromShamsiMonthAndYear(int year, int month)
        => new(year, month, 1, _pc);

    public static DateTime GetGregorianDateMines2MonthsFromShamsiMonthAndYear(int year, int month)
    {
        int myMonth = month, myYear = year;

        if (month > 2) myMonth = month - 2;
        else
        {
            myYear = year - 1;
            myMonth = month switch { 1 => 11, 2 => 12, _ => myMonth };
        }

        return new DateTime(myYear, myMonth, 1, _pc);
    }

    public static DateTime GetGregorianDatePlus2MonthsFromShamsiMonthAndYear(int year, int month)
    {
        int myMonth = month, myYear = year;

        if (month < 11) myMonth = month + 2;
        else
        {
            myYear = year + 1;
            myMonth = month switch { 11 => 1, 12 => 2, _ => myMonth };
        }

        return new DateTime(myYear, myMonth, 1, _pc);
    }

    /// <summary>ورودی مثل "1403/05/12" یا "1403/5/12"</summary>
    public static DateTime GetDateTimeFromShamsiString(string shamsiDate)
    {
        var parts = shamsiDate.ConvertToEn().Split('/');
        return new DateTime(
            int.Parse(parts[0]),
            int.Parse(parts[1]),
            int.Parse(parts[2]),
            _pc);
    }

    /// <summary>تاریخ شمسی امروز به شکل yyyy/MM/dd</summary>
    public static string TodayShamsi() => DateTime.Now.GetShamsiDateFromGregorian(time: false);

    /// <summary>فاصله ماه بین دو تاریخ (برای محاسبه شهریه)</summary>
    public static int MonthsBetweenShamsi(DateTime from, DateTime to)
    {
        int y1 = _pc.GetYear(from), m1 = _pc.GetMonth(from);
        int y2 = _pc.GetYear(to), m2 = _pc.GetMonth(to);
        return Math.Max(0, (y2 - y1) * 12 + (m2 - m1) + 1);
    }
}