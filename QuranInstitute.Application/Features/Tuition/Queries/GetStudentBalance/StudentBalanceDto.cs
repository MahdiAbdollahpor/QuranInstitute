namespace QuranInstitute.Application.Features.Tuition.Queries.GetStudentBalance;

public class StudentBalanceDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
    public string ParentMobile { get; set; } = string.Empty;
    public Guid? ClassId { get; set; }
    public string ClassName { get; set; } = "—";
    public decimal MonthlyFee { get; set; }
    public DateTime StartDate { get; set; }
    public int MonthCount { get; set; }
    public decimal TotalOwed { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Balance { get; set; }
}