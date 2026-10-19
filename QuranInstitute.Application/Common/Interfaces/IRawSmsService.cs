namespace QuranInstitute.Application.Common.Interfaces;

/// <summary>
/// ارسال خام SMS — بدون لاگ.
/// فقط توسط ISmsService استفاده می‌شود.
/// </summary>
public interface IRawSmsService
{
    Task<bool> SendAsync(string mobile, string message, CancellationToken ct = default);
    Task<int> SendBulkAsync(IEnumerable<string> mobiles, string message, CancellationToken ct = default);
}