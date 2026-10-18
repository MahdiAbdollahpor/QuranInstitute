using QuranInstitute.Application.Features.Reports.Queries.GetMonthlyAttendanceReport;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuranInstitute.Web.ViewModels.Reports;

public class AttendanceReportViewModel
{
    public MonthlyAttendanceDto Data { get; set; } = new();
    public int SelectedYear { get; set; }
    public int SelectedMonth { get; set; }
    public Guid? ClassId { get; set; }
    public List<SelectListItem> Classes { get; set; } = new();
}