using QuranInstitute.Application.Features.Reports.Queries.GetDashboardStats;

namespace QuranInstitute.Web.ViewModels.Reports;

public class ReportsIndexViewModel
{
    public DashboardStatsDto Stats { get; set; } = new();
    public int CurrentJalaliYear { get; set; }
    public int CurrentJalaliMonth { get; set; }
    public string CurrentMonthName { get; set; } = string.Empty;
}