namespace QuranInstitute.Application.Features.Reports.Queries.GetMonthlyAttendanceReport;

public class MonthlyAttendanceDto
{
    public int JalaliYear { get; set; }
    public int JalaliMonth { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public List<ClassAttendanceReport> Classes { get; set; } = new();
}

public class ClassAttendanceReport
{
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public int SessionsRecorded { get; set; }
    public List<StudentAttendanceRow> Students { get; set; } = new();
}

public class StudentAttendanceRow
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public int LateCount { get; set; }
    public int ExcusedCount { get; set; }
    public int Total { get; set; }
    public double AttendanceRate { get; set; } // درصد
}