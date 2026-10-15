namespace QuranInstitute.Application.Common.Interfaces;

public interface ISmsService
{
    Task<bool> SendAsync(string mobile, string message, CancellationToken ct = default);
    Task<int> SendBulkAsync(IEnumerable<string> mobiles, string message, CancellationToken ct = default);
}