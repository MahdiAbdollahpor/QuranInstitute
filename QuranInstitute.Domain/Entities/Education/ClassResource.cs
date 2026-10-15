using QuranInstitute.Domain.Common;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Domain.Entities.Education;

public class ClassResource : BaseEntity
{
    public Guid ClassId { get; set; }
    public Class Class { get; set; } = null!;

    public Guid? SessionId { get; set; }                    // اگر به جلسه خاصی وابسته باشد
    public ClassSession? Session { get; set; }

    public ResourceType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FileUrl { get; set; } = string.Empty;     // مسیر فایل یا URL خارجی
    public long? FileSize { get; set; }
    public string? MimeType { get; set; }

    /// <summary>اگر true باشد، فقط شاگردان ثبت‌نام‌شده می‌بینند.</summary>
    public bool EnrolledOnly { get; set; } = true;

    public Guid UploadedBy { get; set; }
}