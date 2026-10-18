namespace QuranInstitute.Application.Features.Reports.Queries.GetDashboardStats;

public class DashboardStatsDto
{
    public int ActiveStudents { get; set; }
    public int ActiveClasses { get; set; }
    public int PendingEnrollments { get; set; }
    public decimal MonthlyIncome { get; set; }
    public decimal TotalDebt { get; set; }
    public int DebtorsCount { get; set; }
    public int TodaysAbsentees { get; set; }
    public int ActiveSessionsThisMonth { get; set; }
}