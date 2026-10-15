using QuranInstitute.Domain.Common;

namespace QuranInstitute.Domain.Entities.Communication;

public class Announcement : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;

    /// <summary>اگر خالی باشد، برای همه است.</summary>
    public Guid? TargetClassId { get; set; }

    /// <summary>اگر مقدار داشته باشد، فقط برای این شاگرد است.</summary>
    public Guid? TargetStudentId { get; set; }

    public bool SendSms { get; set; }
    public bool SendWhatsApp { get; set; }
    public bool SendInApp { get; set; } = true;

    public DateTime? SentAt { get; set; }
    public Guid CreatedByUserId { get; set; }
}