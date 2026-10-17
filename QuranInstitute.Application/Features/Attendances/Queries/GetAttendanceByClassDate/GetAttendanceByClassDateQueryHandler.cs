using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Attendances.Queries.GetAttendanceByClassDate;

public class GetAttendanceByClassDateQueryHandler
    : IRequestHandler<GetAttendanceByClassDateQuery, Result<AttendanceFormResult>>
{
    private readonly IApplicationDbContext _db;

    public GetAttendanceByClassDateQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<AttendanceFormResult>> Handle(
        GetAttendanceByClassDateQuery request, CancellationToken ct)
    {
        // ─── مرحله ۱: داده خام کلاس ───
        var cls = await _db.Classes
            .AsNoTracking()
            .Where(c => c.Id == request.ClassId)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.TeacherName,
                c.DaysCsv
            })
            .FirstOrDefaultAsync(ct);

        if (cls is null)
            return Result<AttendanceFormResult>.Failure("کلاس یافت نشد.");

        var days = string.IsNullOrWhiteSpace(cls.DaysCsv)
            ? Array.Empty<int>()
            : cls.DaysCsv.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse).ToArray();

        // ─── مرحله ۲: نرمال‌سازی تاریخ (فقط date) ───
        var date = request.Date.Date;

        // ─── مرحله ۳: شاگردان فعال کلاس ───
        var students = await _db.Enrollments
            .AsNoTracking()
            .Where(e => e.ClassId == request.ClassId
                     && e.Status == EnrollmentStatus.Approved)
            .OrderBy(e => e.Student.Name)
            .Select(e => new
            {
                StudentId = e.StudentId,
                Name = e.Student.Name,
                Mobile = e.Student.ParentMobile
            })
            .ToListAsync(ct);

        // ─── مرحله ۴: رکوردهای حضور ثبت‌شده در آن تاریخ ───
        var existing = await _db.Attendances
            .AsNoTracking()
            .Where(a => a.ClassId == request.ClassId && a.Date == date)
            .ToDictionaryAsync(a => a.StudentId, ct);

        // ─── مرحله ۵: ساخت لیست ورودی ───
        var entries = students.Select(s =>
        {
            existing.TryGetValue(s.StudentId, out var att);

            return new AttendanceEntryDto
            {
                StudentId = s.StudentId,
                StudentName = s.Name,
                ParentMobile = s.Mobile,
                Status = att?.Status,
                Note = att?.Note
            };
        }).ToList();

        // ─── مرحله ۶: بررسی اینکه آیا این روز جزو روزهای کلاس است ───
        var wd = (int)date.DayOfWeek;  // 0=Sunday ... 6=Saturday
        var isClassDay = days.Contains(wd);

        var result = new AttendanceFormResult
        {
            ClassId = cls.Id,
            ClassName = cls.Name,
            TeacherName = cls.TeacherName,
            Days = days,
            Date = date,
            IsClassDay = isClassDay,
            AlreadyRecorded = existing.Count > 0,
            Entries = entries
        };

        return Result<AttendanceFormResult>.Success(result);
    }
}