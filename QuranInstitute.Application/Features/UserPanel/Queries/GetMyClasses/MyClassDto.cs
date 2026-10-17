using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.UserPanel.Queries.GetMyClasses;

public class MyClassDto
{
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public string? ClassTime { get; set; }
    public int[] Days { get; set; } = Array.Empty<int>();
    public ClassType Type { get; set; }
    public string? OnlineLink { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string EnrollmentStatus { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
    public int TotalSessions { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
}