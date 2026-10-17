using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Attendances.Queries.GetAttendanceByClassDate;

public class AttendanceEntryDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string ParentMobile { get; set; } = string.Empty;

    /// <summary>وضعیت فعلی (اگر ثبت نشده باشد Null است)</summary>
    public AttendanceStatus? Status { get; set; }

    public string? Note { get; set; }
}
