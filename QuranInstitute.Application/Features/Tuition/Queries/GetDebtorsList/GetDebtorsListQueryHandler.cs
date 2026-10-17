using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Application.Features.Tuition.Services;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Tuition.Queries.GetDebtorsList;

public class GetDebtorsListQueryHandler
    : IRequestHandler<GetDebtorsListQuery, PagedList<DebtorItemDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ITuitionCalculator _calculator;

    public GetDebtorsListQueryHandler(
        IApplicationDbContext db,
        ITuitionCalculator calculator)
    {
        _db = db;
        _calculator = calculator;
    }

    public async Task<PagedList<DebtorItemDto>> Handle(
        GetDebtorsListQuery request, CancellationToken ct)
    {
        var q = _db.Students
            .AsNoTracking()
            .Where(s => s.Active);

        if (request.ClassId.HasValue)
        {
            q = q.Where(s => s.Enrollments.Any(e =>
                e.ClassId == request.ClassId.Value &&
                e.Status == EnrollmentStatus.Approved));
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            q = q.Where(x =>
                x.Name.Contains(s) ||
                x.ParentName.Contains(s) ||
                x.ParentMobile.Contains(s));
        }

        var students = await q
            .Select(s => new
            {
                s.Id,
                s.Name,
                s.ParentName,
                s.ParentMobile,
                s.MonthlyFee,
                s.StartDate,
                ClassName = s.Enrollments
                    .Where(e => e.Status == EnrollmentStatus.Approved)
                    .Select(e => e.Class.Name)
                    .FirstOrDefault()
            })
            .ToListAsync(ct);

        // محاسبه بدهی برای همه (در حافظه) — چون محاسبه به تاریخ امروز وابسته است
        var allResults = new List<DebtorItemDto>();

        foreach (var s in students)
        {
            var calc = await _calculator.CalculateAsync(s.Id, ct);

            if (calc.Balance <= 0) continue;   // فقط بدهکاران

            allResults.Add(new DebtorItemDto
            {
                StudentId = s.Id,
                StudentName = s.Name,
                ParentName = s.ParentName,
                ParentMobile = s.ParentMobile,
                ClassName = s.ClassName ?? "—",
                MonthlyFee = s.MonthlyFee,
                MonthCount = calc.MonthCount,
                TotalOwed = calc.TotalOwed,
                TotalPaid = calc.TotalPaid,
                Balance = calc.Balance
            });
        }

        var total = allResults.Count;
        var sorted = allResults
            .OrderByDescending(x => x.Balance)
            .ThenBy(x => x.StudentName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new PagedList<DebtorItemDto>
        {
            Items = sorted,
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}