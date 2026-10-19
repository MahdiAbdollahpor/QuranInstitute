using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Common.Interfaces;

public class SmsSendRequest
{
    public string Mobile { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Guid? StudentId { get; set; }
    public string? RecipientName { get; set; }
    public SmsType Type { get; set; } = SmsType.Manual;
}

/// <summary>
/// ارسال SMS با ثبت خودکار در تاریخچه.
/// در همه Commandها از این Interface استفاده کن.
/// </summary>
public interface ISmsService
{
    Task<bool> SendAsync(
        string mobile,
        string message,
        SmsType type = SmsType.Manual,
        Guid? studentId = null,
        string? recipientName = null,
        CancellationToken ct = default);

    Task<int> SendBulkAsync(
        IEnumerable<SmsSendRequest> requests,
        CancellationToken ct = default);
}