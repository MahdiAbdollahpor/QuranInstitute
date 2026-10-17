using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Entities.Attendances;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Attendances.Commands.BulkRecordAttendance;

public class BulkRecordAttendanceCommandHandler
    : IRequestHandler<BulkRecordAttendanceCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _current;

    public BulkRecordAttendanceCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService current)
    {
        _db = db;
        _current = current;
    }

    public async Task<Result> Handle(
        BulkRecordAttendanceCommand request, CancellationToken ct)
    {
        var date = request.Date.Date;

        // بررسی وجود کلاس
        var clsExists = await _db.Classes
            .AnyAsync(c => c.Id == request.ClassId, ct);

        if (!clsExists)
            return Result.Failure("کلاس یافت نشد.");

        // بررسی شاگردان مجاز
        var allowedStudentIds = await _db.Enrollments
            .Where(e => e.ClassId == request.ClassId
                     && e.Status == EnrollmentStatus.Approved)
            .Select(e => e.StudentId)
            .ToListAsync(ct);

        var invalid = request.Items
            .Where(i => !allowedStudentIds.Contains(i.StudentId))
            .ToList();

        if (invalid.Any())
            return Result.Failure("برخی شاگردان در این کلاس ثبت‌نام نشده‌اند.");

        // گرفتن رکوردهای موجود
        var existing = await _db.Attendances
            .Where(a => a.ClassId == request.ClassId && a.Date == date)
            .ToListAsync(ct);

        var existingByStudent = existing.ToDictionary(a => a.StudentId);
        var userId = _current.UserId;

        foreach (var item in request.Items)
        {
            if (existingByStudent.TryGetValue(item.StudentId, out var att))
            {
                // به‌روزرسانی
                att.Status = item.Status;
                att.Note = item.Note?.Trim();
                att.UpdatedAt = DateTime.UtcNow;
                att.UpdatedBy = userId;
            }
            else
            {
                // ایجاد
                _db.Attendances.Add(new Attendance
                {
                    ClassId = request.ClassId,
                    StudentId = item.StudentId,
                    Date = date,
                    Status = item.Status,
                    Note = item.Note?.Trim(),
                    RecordedBy = userId ?? Guid.Empty
                });
            }
        }

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}