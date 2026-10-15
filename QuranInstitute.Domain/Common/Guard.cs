namespace QuranInstitute.Domain.Common;

/// <summary>
/// ابزار اعتبارسنجی سریع در لایه دامنه.
/// فقط برای قواعد ابتدایی؛ اعتبارسنجی کامل در FluentValidation است.
/// </summary>
public static class Guard
{
    public static void AgainstNull(object? value, string name)
    {
        if (value is null)
            throw new DomainException($"{name} نمی‌تواند خالی باشد.");
    }

    public static void AgainstEmpty(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException($"{name} نمی‌تواند خالی باشد.");
    }

    public static void AgainstNegative(decimal value, string name)
    {
        if (value < 0)
            throw new DomainException($"{name} نمی‌تواند منفی باشد.");
    }

    public static void AgainstOutOfRange(int value, int min, int max, string name)
    {
        if (value < min || value > max)
            throw new DomainException($"{name} باید بین {min} و {max} باشد.");
    }
}