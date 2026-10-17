using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Entities.Education;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.UserPanel.Commands.RequestEnrollment;

public class RequestMyEnrollmentCommandHandler
    : IRequestHandler<RequestMyEnrollmentCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _current;

    public RequestMyEnrollmentCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService current)
    {
        _db = db;
        _current = current;
    }

    public async Task<Result> Handle(
        RequestMyEnrollmentCommand request, CancellationToken ct)
    {
        var userId = _current.UserId;
        if (!userId.HasValue)
            return Result.Failure("کاربر شناسایی نشد.");

        // بررسی مالکیت شاگرد
        var student = await _db.Students
            .FirstOrDefaultAsync(s => s.Id == request.StudentId
                                   && s.UserId == userId.Value && s.Active, ct);

        if (student is null)
            return Result.Failure("شاگرد یافت نشد یا دسترسی ندارید.");

        // بررسی کلاس
        var cls = await _db.Classes
            .AsNoTracking()
            .Where(c => c.Id == request.ClassId && c.Active)
            .Select(c => new { c.Id, c.Name, c.Capacity })
            .FirstOrDefaultAsync(ct);

        if (cls is null)
            return Result.Failure("کلاس یافت نشد.");

        // بررسی تکراری نبودن
        var exists = await _db.Enrollments.AnyAsync(e =>
            e.StudentId == request.StudentId &&
            e.ClassId == request.ClassId &&
            (e.Status == EnrollmentStatus.Approved ||
             e.Status == EnrollmentStatus.Pending), ct);

        if (exists)
            return Result.Failure("قبلاً برای این کلاس درخواست داده‌اید یا ثبت‌نام شده‌اید.");

        // بررسی ظرفیت
        if (cls.Capacity > 0)
        {
            var approved = await _db.Enrollments
                .CountAsync(e => e.ClassId == request.ClassId
                              && e.Status == EnrollmentStatus.Approved, ct);
            if (approved >= cls.Capacity)
                return Result.Failure("ظرفیت این کلاس پر است.");
        }

        _db.Enrollments.Add(new Enrollment
        {
            StudentId = request.StudentId,
            ClassId = request.ClassId,
            Status = EnrollmentStatus.Pending,
            RequestedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}