namespace QuranInstitute.Application.Features.Reports.Queries.GetFinancialSummary;

public class FinancialSummaryDto
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }

    public decimal TotalIncome { get; set; }
    public int PaymentCount { get; set; }
    public decimal AveragePayment { get; set; }

    public List<PaymentMethodBreakdown> ByMethod { get; set; } = new();
    public List<DailyIncome> DailyBreakdown { get; set; } = new();
}

public class PaymentMethodBreakdown
{
    public string MethodName { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int Count { get; set; }
    public double Percent { get; set; }
}

public class DailyIncome
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public int Count { get; set; }
}