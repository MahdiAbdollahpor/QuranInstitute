namespace QuranInstitute.Domain.Common;

/// <summary>
/// خطای مربوط به قواعد دامنه (Business Rule).
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception inner) : base(message, inner) { }
}