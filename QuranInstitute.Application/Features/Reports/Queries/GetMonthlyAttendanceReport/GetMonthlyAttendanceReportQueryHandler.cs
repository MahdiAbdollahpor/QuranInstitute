using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;
using QuranInstitute.Shared.Helpers;

namespace QuranInstitute.Application.Features.Reports.Queries.GetMonthlyAttendanceReport;

public class GetMonthlyAttendanceReportQueryHandler
    : IRequestHandler<GetMonthlyAttendanceReportQuery, Result<MonthlyAttendanceDto>>
{
    private readonly IApplicationDbContext _db;

    private static readonly string[] MonthNames =
    {
        "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
        "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
    };

    public GetMonthlyAttendanceReportQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<MonthlyAttendanceDto>> Handle(
        GetMonthlyAttendanceReportQuery request, CancellationToken ct)
    {
        // تبدیل ماه شمسی به بازه میلادی
        var fromDate = MyDateTime.GetGregorianDateFromShamsiMonthAndYear(
            request.JalaliYear, request.JalaliMonth);
        var toDate = fromDate.AddMonths(1);

        // کلاس‌ها
        var classesQuery = _db.Classes
            .AsNoTracking()
            .Where(c => c.Active);

        if (request.ClassId.HasValue)
            classesQuery = classesQuery.Where(c => c.Id == request.ClassId.Value);

        var classes = await classesQuery
            .Select(c => new { c.Id, c.Name, c.TeacherName })
            .ToListAsync(ct);

        var classIds = classes.Select(c => c.Id).ToList();

        // شاگردان هر کلاس
        var enrollments = await _db.Enrollments
            .AsNoTracking()
            .Where(e => classIds.Contains(e.ClassId)
                     && e.Status == EnrollmentStatus.Approved)
            .Select(e => new { e.ClassId, e.StudentId, StudentName = e.Student.Name })
            .ToListAsync(ct);

        // حضور و غیاب در بازه
        var attendances = await _db.Attendances
            .AsNoTracking()
            .Where(a => classIds.Contains(a.ClassId)
                     && a.Date >= fromDate && a.Date < toDate)
            .Select(a => new { a.ClassId, a.StudentId, a.Status, a.Date })
            .ToListAsync(ct);

        // ساخت گزارش
        var result = new MonthlyAttendanceDto
        {
            JalaliYear = request.JalaliYear,
            JalaliMonth = request.JalaliMonth,
            MonthName = MonthNames[request.JalaliMonth - 1]
        };

        foreach (var cls in classes)
        {
            var classStudents = enrollments.Where(e => e.ClassId == cls.Id).ToList();
            if (!classStudents.Any()) continue;

            var classAtt = attendances.Where(a => a.ClassId == cls.Id).ToList();

            // تعداد جلسات ثبت‌شده = تعداد تاریخ‌های یکتا
            var sessionCount = classAtt.Select(a => a.Date.Date).Distinct().Count();

            var report = new ClassAttendanceReport
            {
                ClassId = cls.Id,
                ClassName = cls.Name,
                TeacherName = cls.TeacherName,
                TotalStudents = classStudents.Count,
                SessionsRecorded = sessionCount
            };

            foreach (var s in classStudents)
            {
                var sAtt = classAtt.Where(a => a.StudentId == s.StudentId).ToList();

                var present = sAtt.Count(a => a.Status == AttendanceStatus.Present);
                var absent = sAtt.Count(a => a.Status == AttendanceStatus.Absent);
                var late = sAtt.Count(a => a.Status == AttendanceStatus.Late);
                var excused = sAtt.Count(a => a.Status == AttendanceStatus.Excused);
                var total = present + absent + late + excused;

                var rate = total > 0
                    ? (double)(present + late) / total * 100
                    : 0;

                report.Students.Add(new StudentAttendanceRow
                {
                    StudentId = s.StudentId,
                    StudentName = s.StudentName,
                    PresentCount = present,
                    AbsentCount = absent,
                    LateCount = late,
                    ExcusedCount = excused,
                    Total = total,
                    AttendanceRate = Math.Round(rate, 1)
                });
            }

            result.Classes.Add(report);
        }

        return Result<MonthlyAttendanceDto>.Success(result);
    }
}