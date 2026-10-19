using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Notifications.Queries.GetSmsHistory;

public class SmsHistoryItemDto
{
    public Guid Id { get; set; }
    public Guid? StudentId { get; set; }
    public string? StudentName { get; set; }
    public string Mobile { get; set; } = string.Empty;
    public string? RecipientName { get; set; }
    public string Message { get; set; } = string.Empty;
    public SmsType Type { get; set; }
    public SmsStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ProviderMessageId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public DateTime? SentAt { get; set; }
    public DateTime CreatedAt { get; set; }
}