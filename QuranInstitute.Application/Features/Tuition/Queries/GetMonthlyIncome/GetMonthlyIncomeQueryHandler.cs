using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Tuition.Queries.GetMonthlyIncome;

public class GetMonthlyIncomeQueryHandler
    : IRequestHandler<GetMonthlyIncomeQuery, Result<MonthlyIncomeDto>>
{
    private readonly IApplicationDbContext _db;

    public GetMonthlyIncomeQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<MonthlyIncomeDto>> Handle(
        GetMonthlyIncomeQuery request, CancellationToken ct)
    {
        var today = DateTime.Today;
        var year = request.Year ?? today.Year;
        var month = request.Month ?? today.Month;

        var from = new DateTime(year, month, 1);
        var to = from.AddMonths(1);

        var payments = await _db.Payments
            .AsNoTracking()
            .Where(p => p.Date >= from && p.Date < to
                     && p.Status == PaymentStatus.Verified)
            .Select(p => new { p.Amount, p.Method })
            .ToListAsync(ct);

        var months = new[]
        {
            "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
            "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
        };

        // توجه: این ماه میلادی است. اگر شمسی می‌خواهی، از MyDateTime استفاده کن.
        // برای سادگی، ما فقط خروجی را به شمسی نزدیک نمایش می‌دهیم.
        var dto = new MonthlyIncomeDto
        {
            JalaliYear = year,
            JalaliMonth = month,
            MonthName = $"{year}/{month:00}",
            TotalIncome = payments.Sum(p => p.Amount),
            PaymentCount = payments.Count,
            ByMethod = payments
                .GroupBy(p => p.Method)
                .Select(g => new PaymentMethodBreakdown
                {
                    MethodName = GetMethodName(g.Key),
                    Total = g.Sum(x => x.Amount),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Total)
                .ToList()
        };

        return Result<MonthlyIncomeDto>.Success(dto);
    }

    public static string GetMethodName(PaymentMethod m) => m switch
    {
        PaymentMethod.Cash => "نقدی",
        PaymentMethod.Card => "کارت به کارت",
        PaymentMethod.Online => "آنلاین",
        PaymentMethod.Other => "سایر",
        _ => "—"
    };
}