using Microsoft.Extensions.Logging;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Domain.Entities.Communication;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Infrastructure.Services.Sms;

/// <summary>
/// ارسال SMS + ثبت خودکار در تاریخچه (SmsLog).
/// از IRawSmsService برای ارسال واقعی استفاده می‌کند.
/// </summary>
public class SmsServiceWithLogging : ISmsService
{
    private readonly IRawSmsService _raw;
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _current;
    private readonly ILogger<SmsServiceWithLogging> _logger;

    public SmsServiceWithLogging(
        IRawSmsService raw,
        IApplicationDbContext db,
        ICurrentUserService current,
        ILogger<SmsServiceWithLogging> logger)
    {
        _raw = raw;
        _db = db;
        _current = current;
        _logger = logger;
    }

    public async Task<bool> SendAsync(
        string mobile,
        string message,
        SmsType type = SmsType.Manual,
        Guid? studentId = null,
        string? recipientName = null,
        CancellationToken ct = default)
    {
        var normalizedMobile = NormalizeMobile(mobile);

        var log = new SmsLog
        {
            StudentId = studentId,
            Mobile = normalizedMobile,
            RecipientName = recipientName,
            Message = message.Length > 2000 ? message.Substring(0, 2000) : message,
            Type = type,
            Status = SmsStatus.Pending,
            Provider = GetProviderName(),
            SentByUserId = _current.UserId
        };

        _db.SmsLogs.Add(log);
        await _db.SaveChangesAsync(ct);

        try
        {
            var success = await _raw.SendAsync(normalizedMobile, message, ct);

            log.Status = success ? SmsStatus.Sent : SmsStatus.Failed;
            log.SentAt = success ? DateTime.UtcNow : null;
            if (!success)
                log.ErrorMessage = "ارسال ناموفق از سمت درگاه";

            await _db.SaveChangesAsync(ct);
            return success;
        }
        catch (Exception ex)
        {
            log.Status = SmsStatus.Failed;
            log.ErrorMessage = ex.Message.Length > 1000
                ? ex.Message.Substring(0, 1000)
                : ex.Message;

            await _db.SaveChangesAsync(ct);
            _logger.LogError(ex, "SMS sending failed to {Mobile}", mobile);
            return false;
        }
    }

    public async Task<int> SendBulkAsync(
        IEnumerable<SmsSendRequest> requests, CancellationToken ct = default)
    {
        int successCount = 0;

        foreach (var r in requests)
        {
            if (await SendAsync(
                r.Mobile, r.Message, r.Type, r.StudentId, r.RecipientName, ct))
                successCount++;
        }

        return successCount;
    }

    private static string NormalizeMobile(string mobile)
    {
        var n = mobile.Trim().Replace(" ", "").Replace("-", "");
        if (n.StartsWith("+98")) n = "0" + n.Substring(3);
        if (n.StartsWith("0098")) n = "0" + n.Substring(4);
        if (n.StartsWith("98") && n.Length == 12) n = "0" + n.Substring(2);
        return n;
    }

    private string GetProviderName()
    {
        var name = _raw.GetType().Name;
        return name.Replace("SmsService", "").Replace("Service", "").TrimEnd() ?? "Unknown";
    }
}