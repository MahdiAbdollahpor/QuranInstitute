namespace QuranInstitute.Application.Features.Tuition.Services;

public interface ITuitionCalculator
{
    /// <summary>
    /// محاسبه بدهی/بستانکاری یک شاگرد.
    /// فرمول: شهریه ماهانه × تعداد ماه گذشته از تاریخ شروع (شامل ماه جاری) - مجموع پرداخت‌ها
    /// </summary>
    Task<StudentBalanceResult> CalculateAsync(Guid studentId, CancellationToken ct = default);
}

public class StudentBalanceResult
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public decimal MonthlyFee { get; set; }
    public DateTime StartDate { get; set; }
    public int MonthCount { get; set; }
    public decimal TotalOwed { get; set; }
    public decimal TotalPaid { get; set; }

    /// <summary>مثبت = بدهکار، منفی = بستانکار، صفر = تسویه</summary>
    public decimal Balance { get; set; }
}