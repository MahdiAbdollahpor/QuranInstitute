using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.UserPanel.Queries.GetMyAttendance;

public class MyAttendanceDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public List<AttendanceRowDto> Rows { get; set; } = new();

    public int PresentCount => Rows.Count(r => r.Status == AttendanceStatus.Present);
    public int AbsentCount => Rows.Count(r => r.Status == AttendanceStatus.Absent);
    public int LateCount => Rows.Count(r => r.Status == AttendanceStatus.Late);
    public int ExcusedCount => Rows.Count(r => r.Status == AttendanceStatus.Excused);
}

public class AttendanceRowDto
{
    public DateTime Date { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public AttendanceStatus Status { get; set; }
    public string? Note { get; set; }
}