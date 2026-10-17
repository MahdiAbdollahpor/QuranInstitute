using QuranInstitute.Application.Features.UserPanel.Queries.GetMyProfile;

namespace QuranInstitute.Application.Features.UserPanel.Queries.GetMyDashboard;

public class MyDashboardDto
{
    public List<MyStudentInfo> Students { get; set; } = new();
    public decimal TotalDebt { get; set; }
    public decimal TotalPaidThisMonth { get; set; }
    public int AbsencesThisMonth { get; set; }
    public int UnreadNotifications { get; set; }
    public List<UpcomingClassDto> UpcomingClasses { get; set; } = new();
}

public class UpcomingClassDto
{
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public int[] Days { get; set; } = Array.Empty<int>();
    public string? ClassTime { get; set; }
    public string? OnlineLink { get; set; }
}