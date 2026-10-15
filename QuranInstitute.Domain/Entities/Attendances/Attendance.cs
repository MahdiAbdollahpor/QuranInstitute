using QuranInstitute.Domain.Common;
using QuranInstitute.Domain.Entities.Education;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Domain.Entities.Attendances;

public class Attendance : BaseEntity
{
    public Guid ClassId { get; set; }
    public Class Class { get; set; } = null!;

    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;

    /// <summary>تاریخ ثبت (فقط بخش تاریخ مهم است، زمان بی‌اهمیت)</summary>
    public DateTime Date { get; set; }

    public AttendanceStatus Status { get; set; }
    public string? Note { get; set; }
    public Guid RecordedBy { get; set; }
}