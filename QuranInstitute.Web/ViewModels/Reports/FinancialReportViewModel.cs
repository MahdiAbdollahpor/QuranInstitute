using QuranInstitute.Application.Features.Reports.Queries.GetFinancialSummary;

namespace QuranInstitute.Web.ViewModels.Reports;

public class FinancialReportViewModel
{
    public FinancialSummaryDto Data { get; set; } = new();
    public int SelectedYear { get; set; }
    public int SelectedMonth { get; set; }
}