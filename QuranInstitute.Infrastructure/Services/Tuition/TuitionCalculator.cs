using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Features.Tuition.Services;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Infrastructure.Services.Tuition;

public class TuitionCalculator : ITuitionCalculator
{
    private readonly IApplicationDbContext _db;

    public TuitionCalculator(IApplicationDbContext db) => _db = db;

    public async Task<StudentBalanceResult> CalculateAsync(
        Guid studentId, CancellationToken ct = default)
    {
        var student = await _db.Students
            .AsNoTracking()
            .Where(s => s.Id == studentId)
            .Select(s => new
            {
                s.Id,
                s.Name,
                s.MonthlyFee,
                s.StartDate
            })
            .FirstOrDefaultAsync(ct)
            ?? throw new InvalidOperationException("شاگرد یافت نشد.");

        var totalPaid = await _db.Payments
            .AsNoTracking()
            .Where(p => p.StudentId == studentId
                     && p.Status == PaymentStatus.Verified)
            .SumAsync(p => (decimal?)p.Amount, ct) ?? 0m;

        var monthCount = CalculateMonthCount(student.StartDate, DateTime.Today);
        var totalOwed = student.MonthlyFee * monthCount;

        return new StudentBalanceResult
        {
            StudentId = student.Id,
            StudentName = student.Name,
            MonthlyFee = student.MonthlyFee,
            StartDate = student.StartDate,
            MonthCount = monthCount,
            TotalOwed = totalOwed,
            TotalPaid = totalPaid,
            Balance = totalOwed - totalPaid
        };
    }

    /// <summary>
    /// محاسبه تعداد ماه بین دو تاریخ (شامل ماه جاری).
    /// اگر StartDate در آینده باشد، صفر برمی‌گرداند.
    /// </summary>
    private static int CalculateMonthCount(DateTime start, DateTime today)
    {
        if (start > today) return 0;

        var months = (today.Year - start.Year) * 12
                   + (today.Month - start.Month)
                   + 1;  // شامل ماه جاری

        return Math.Max(0, months);
    }
}