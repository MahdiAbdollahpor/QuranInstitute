namespace QuranInstitute.Application.Features.Tuition.Queries.GetMonthlyIncome;

public class MonthlyIncomeDto
{
    public int JalaliYear { get; set; }
    public int JalaliMonth { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public decimal TotalIncome { get; set; }
    public int PaymentCount { get; set; }
    public List<PaymentMethodBreakdown> ByMethod { get; set; } = new();
}

public class PaymentMethodBreakdown
{
    public string MethodName { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int Count { get; set; }
}