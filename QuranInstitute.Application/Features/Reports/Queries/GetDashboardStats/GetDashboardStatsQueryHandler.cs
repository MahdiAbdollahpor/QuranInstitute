using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Reports.Queries.GetDashboardStats;

public class GetDashboardStatsQueryHandler
    : IRequestHandler<GetDashboardStatsQuery, Result<DashboardStatsDto>>
{
    private readonly IApplicationDbContext _db;

    public GetDashboardStatsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<DashboardStatsDto>> Handle(
        GetDashboardStatsQuery request, CancellationToken ct)
    {
        var today = DateTime.Today;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var monthEnd = monthStart.AddMonths(1);

        var stats = new DashboardStatsDto();

        stats.ActiveStudents = await _db.Students
            .CountAsync(s => s.Active, ct);

        stats.ActiveClasses = await _db.Classes
            .CountAsync(c => c.Active, ct);

        stats.PendingEnrollments = await _db.Enrollments
            .CountAsync(e => e.Status == EnrollmentStatus.Pending, ct);

        stats.MonthlyIncome = await _db.Payments
            .Where(p => p.Status == PaymentStatus.Verified
                     && p.Date >= monthStart && p.Date < monthEnd)
            .SumAsync(p => (decimal?)p.Amount, ct) ?? 0m;

        stats.TodaysAbsentees = await _db.Attendances
            .CountAsync(a => a.Date == today && a.Status == AttendanceStatus.Absent, ct);

        stats.ActiveSessionsThisMonth = await _db.ClassSessions
            .CountAsync(s => s.Date >= monthStart && s.Date < monthEnd, ct);

        return Result<DashboardStatsDto>.Success(stats);
    }
}