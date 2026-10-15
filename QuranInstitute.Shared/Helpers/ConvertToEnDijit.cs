using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuranInstitute.Shared.Helpers;

public static class ConvertToEnDijit
{
    private static readonly string[] PersianDigits =
        { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };

    private static readonly string[] ArabicDigits =
        { "٠", "١", "٢", "٣", "٤", "٥", "٦", "٧", "٨", "٩" };

    public static string ConvertToEn(this string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        for (int i = 0; i < 10; i++)
        {
            input = input.Replace(PersianDigits[i], i.ToString());
            input = input.Replace(ArabicDigits[i], i.ToString());
        }
        return input;
    }

    public static string ConvertToFa(this string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        for (int i = 0; i < 10; i++)
            input = input.Replace(i.ToString(), PersianDigits[i]);
        return input;
    }
}