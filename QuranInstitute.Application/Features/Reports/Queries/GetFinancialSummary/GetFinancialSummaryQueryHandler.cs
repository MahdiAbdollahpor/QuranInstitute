using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;
using QuranInstitute.Shared.Helpers;

namespace QuranInstitute.Application.Features.Reports.Queries.GetFinancialSummary;

public class GetFinancialSummaryQueryHandler
    : IRequestHandler<GetFinancialSummaryQuery, Result<FinancialSummaryDto>>
{
    private readonly IApplicationDbContext _db;

    public GetFinancialSummaryQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<FinancialSummaryDto>> Handle(
        GetFinancialSummaryQuery request, CancellationToken ct)
    {
        var fromDate = MyDateTime.GetGregorianDateFromShamsiMonthAndYear(
            request.JalaliYear, request.JalaliMonth);
        var toDate = fromDate.AddMonths(1);

        var payments = await _db.Payments
            .AsNoTracking()
            .Where(p => p.Status == PaymentStatus.Verified
                     && p.Date >= fromDate && p.Date < toDate)
            .Select(p => new { p.Amount, p.Method, p.Date })
            .ToListAsync(ct);

        var totalIncome = payments.Sum(p => p.Amount);
        var count = payments.Count;

        var methodNames = new Dictionary<PaymentMethod, string>
        {
            { PaymentMethod.Cash, "نقدی" },
            { PaymentMethod.Card, "کارت به کارت" },
            { PaymentMethod.Online, "آنلاین" },
            { PaymentMethod.Other, "سایر" }
        };

        var byMethod = payments
            .GroupBy(p => p.Method)
            .Select(g => new PaymentMethodBreakdown
            {
                MethodName = methodNames[g.Key],
                Total = g.Sum(x => x.Amount),
                Count = g.Count(),
                Percent = totalIncome > 0
                    ? Math.Round((double)g.Sum(x => x.Amount) / (double)totalIncome * 100, 1)
                    : 0
            })
            .OrderByDescending(x => x.Total)
            .ToList();

        var daily = payments
            .GroupBy(p => p.Date.Date)
            .Select(g => new DailyIncome
            {
                Date = g.Key,
                Amount = g.Sum(x => x.Amount),
                Count = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToList();

        return Result<FinancialSummaryDto>.Success(new FinancialSummaryDto
        {
            FromDate = fromDate,
            ToDate = toDate.AddDays(-1),
            TotalIncome = totalIncome,
            PaymentCount = count,
            AveragePayment = count > 0 ? totalIncome / count : 0,
            ByMethod = byMethod,
            DailyBreakdown = daily
        });
    }
}