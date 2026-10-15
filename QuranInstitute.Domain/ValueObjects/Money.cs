using QuranInstitute.Domain.Common;

namespace QuranInstitute.Domain.ValueObjects;

/// <summary>
/// مبلغ ریالی — Value Object تغییرناپذیر.
/// </summary>
public sealed class Money : IEquatable<Money>
{
    public decimal Amount { get; }

    private Money(decimal amount) => Amount = amount;

    public static Money FromRial(decimal amount)
    {
        Guard.AgainstNegative(amount, "مبلغ");
        return new Money(amount);
    }

    public static Money Zero => new(0);

    public Money Add(Money other) => new(Amount + other.Amount);
    public Money Subtract(Money other) => new(Amount - other.Amount);

    public override string ToString() => Amount.ToString("N0");
    public bool Equals(Money? other) => other is not null && Amount == other.Amount;
    public override bool Equals(object? obj) => obj is Money m && Equals(m);
    public override int GetHashCode() => Amount.GetHashCode();
}