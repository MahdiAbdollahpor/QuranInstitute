namespace QuranInstitute.Application.Features.UserPanel.Queries.GetMyBalance;

public class MyBalanceDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public decimal MonthlyFee { get; set; }
    public DateTime StartDate { get; set; }
    public int MonthCount { get; set; }
    public decimal TotalOwed { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Balance { get; set; }

    public List<MyPaymentRowDto> Payments { get; set; } = new();
}

public class MyPaymentRowDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string MethodName { get; set; } = string.Empty;
    public string ReceiptNo { get; set; } = string.Empty;
    public string? Note { get; set; }
}