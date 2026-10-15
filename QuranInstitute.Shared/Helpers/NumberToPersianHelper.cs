namespace QuranInstitute.Shared.Helpers;

public static class NumberToPersianHelper
{
    public static string ToPersianDigits(this object? value)
        => value?.ToString().ConvertToFa() ?? string.Empty;

    public static string ToMoney(this decimal amount)
        => amount.ToString("N0").ConvertToFa().Replace(",", "٬") + " تومان";
}