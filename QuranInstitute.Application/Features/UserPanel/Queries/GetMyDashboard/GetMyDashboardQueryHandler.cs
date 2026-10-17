using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Application.Features.Tuition.Services;
using QuranInstitute.Application.Features.UserPanel.Queries.GetMyProfile;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.UserPanel.Queries.GetMyDashboard;

public class GetMyDashboardQueryHandler
    : IRequestHandler<GetMyDashboardQuery, Result<MyDashboardDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _current;
    private readonly ITuitionCalculator _calc;

    public GetMyDashboardQueryHandler(
        IApplicationDbContext db,
        ICurrentUserService current,
        ITuitionCalculator calc)
    {
        _db = db;
        _current = current;
        _calc = calc;
    }

    public async Task<Result<MyDashboardDto>> Handle(
        GetMyDashboardQuery request, CancellationToken ct)
    {
        var userId = _current.UserId;
        if (!userId.HasValue)
            return Result<MyDashboardDto>.Failure("کاربر شناسایی نشد.");

        var students = await _db.Students
            .AsNoTracking()
            .Where(s => s.UserId == userId.Value && s.Active)
            .Select(s => new
            {
                s.Id,
                s.Name,
                s.ParentName,
                s.MonthlyFee,
                s.StartDate,
                ClassName = s.Enrollments
                    .Where(e => e.Status == EnrollmentStatus.Approved)
                    .Select(e => e.Class.Name)
                    .FirstOrDefault()
            })
            .ToListAsync(ct);

        var dto = new MyDashboardDto();

        decimal totalDebt = 0;

        foreach (var s in students)
        {
            dto.Students.Add(new MyStudentInfo
            {
                StudentId = s.Id,
                Name = s.Name,
                ParentName = s.ParentName,
                ClassName = s.ClassName ?? "—",
                MonthlyFee = s.MonthlyFee,
                StartDate = s.StartDate
            });

            var calc = await _calc.CalculateAsync(s.Id, ct);
            if (calc.Balance > 0) totalDebt += calc.Balance;
        }

        dto.TotalDebt = totalDebt;

        // پرداخت‌های این ماه
        var today = DateTime.Today;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var monthEnd = monthStart.AddMonths(1);

        var studentIds = students.Select(s => s.Id).ToList();

        if (studentIds.Any())
        {
            dto.TotalPaidThisMonth = await _db.Payments
                .Where(p => studentIds.Contains(p.StudentId)
                         && p.Status == PaymentStatus.Verified
                         && p.Date >= monthStart && p.Date < monthEnd)
                .SumAsync(p => (decimal?)p.Amount, ct) ?? 0m;

            dto.AbsencesThisMonth = await _db.Attendances
                .CountAsync(a => studentIds.Contains(a.StudentId)
                              && a.Status == AttendanceStatus.Absent
                              && a.Date >= monthStart && a.Date < monthEnd, ct);

            dto.UnreadNotifications = await _db.Notifications
                .CountAsync(n => studentIds.Contains(n.StudentId ?? Guid.Empty)
                              && !n.IsRead, ct);

            // کلاس‌های آینده
            var classes = await _db.Enrollments
                .AsNoTracking()
                .Where(e => studentIds.Contains(e.StudentId)
                         && e.Status == EnrollmentStatus.Approved)
                .Select(e => new
                {
                    e.Class.Id,
                    e.Class.Name,
                    e.Class.TeacherName,
                    e.Class.DaysCsv,
                    e.Class.ClassTime,
                    e.Class.DefaultOnlineLink
                })
                .Distinct()
                .ToListAsync(ct);

            dto.UpcomingClasses = classes.Select(c => new UpcomingClassDto
            {
                ClassId = c.Id,
                ClassName = c.Name,
                TeacherName = c.TeacherName,
                Days = string.IsNullOrWhiteSpace(c.DaysCsv)
                    ? Array.Empty<int>()
                    : c.DaysCsv.Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(int.Parse).ToArray(),
                ClassTime = c.ClassTime,
                OnlineLink = c.DefaultOnlineLink
            }).ToList();
        }

        return Result<MyDashboardDto>.Success(dto);
    }
}