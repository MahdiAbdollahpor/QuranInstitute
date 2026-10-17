namespace QuranInstitute.Application.Features.Tuition.Queries.GetDebtorsList;

public class DebtorItemDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
    public string ParentMobile { get; set; } = string.Empty;
    public string ClassName { get; set; } = "—";
    public decimal MonthlyFee { get; set; }
    public decimal TotalOwed { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Balance { get; set; }
    public int MonthCount { get; set; }
}