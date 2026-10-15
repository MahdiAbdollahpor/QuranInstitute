using QuranInstitute.Domain.Common;
using QuranInstitute.Domain.Entities.Education;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Domain.Entities.Communication;

/// <summary>پیام درون‌برنامه‌ای برای هر کاربر/شاگرد</summary>
public class Notification : BaseEntity
{
    public Guid? UserId { get; set; }
    public Guid? StudentId { get; set; }
    public Student? Student { get; set; }

    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;

    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
}