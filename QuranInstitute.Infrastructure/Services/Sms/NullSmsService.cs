using Microsoft.Extensions.Logging;
using QuranInstitute.Application.Common.Interfaces;

namespace QuranInstitute.Infrastructure.Services.Sms;

/// <summary>
/// پیاده‌سازی موقت که فقط لاگ می‌کند.
/// بعداً با KavenegarSmsService جایگزین می‌شود.
/// </summary>
public class NullSmsService : ISmsService
{
    private readonly ILogger<NullSmsService> _logger;

    public NullSmsService(ILogger<NullSmsService> logger) => _logger = logger;

    public Task<bool> SendAsync(string mobile, string message, CancellationToken ct = default)
    {
        _logger.LogInformation("📱 [SMS-DRY-RUN] به {Mobile}: {Message}", mobile, message);
        return Task.FromResult(true);
    }

    public Task<int> SendBulkAsync(IEnumerable<string> mobiles, string message, CancellationToken ct = default)
    {
        var list = mobiles.ToList();
        _logger.LogInformation("📱 [SMS-DRY-RUN] ارسال به {Count} شماره: {Message}", list.Count, message);
        return Task.FromResult(list.Count);
    }
}