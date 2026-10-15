using System.Text.RegularExpressions;
using QuranInstitute.Domain.Common;

namespace QuranInstitute.Domain.ValueObjects;

/// <summary>
/// شماره موبایل ایران — Value Object تغییرناپذیر.
/// </summary>
public sealed class PhoneNumber : IEquatable<PhoneNumber>
{
    private static readonly Regex Pattern = new(@"^09\d{9}$", RegexOptions.Compiled);

    public string Value { get; }

    private PhoneNumber(string value) => Value = value;

    public static PhoneNumber Create(string input)
    {
        Guard.AgainstEmpty(input, "شماره موبایل");

        var normalized = ToEnglishDigits(input.Trim().Replace("-", "").Replace(" ", ""));

        if (normalized.StartsWith("+98")) normalized = "0" + normalized.Substring(3);
        else if (normalized.StartsWith("0098")) normalized = "0" + normalized.Substring(4);
        else if (normalized.StartsWith("98") && normalized.Length == 12) normalized = "0" + normalized.Substring(2);

        if (!Pattern.IsMatch(normalized))
            throw new DomainException($"شماره موبایل «{input}» معتبر نیست.");

        return new PhoneNumber(normalized);
    }

    private static string ToEnglishDigits(string s)
    {
        var fa = "۰۱۲۳۴۵۶۷۸۹";
        var ar = "٠١٢٣٤٥٦٧٨٩";
        for (int i = 0; i < 10; i++)
            s = s.Replace(fa[i], (char)('0' + i)).Replace(ar[i], (char)('0' + i));
        return s;
    }

    public override string ToString() => Value;
    public bool Equals(PhoneNumber? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => obj is PhoneNumber p && Equals(p);
    public override int GetHashCode() => Value.GetHashCode();
}