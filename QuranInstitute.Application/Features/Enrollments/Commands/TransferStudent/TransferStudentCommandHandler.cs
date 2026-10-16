using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Entities.Education;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Enrollments.Commands.TransferStudent;

public class TransferStudentCommandHandler
    : IRequestHandler<TransferStudentCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _current;

    public TransferStudentCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService current)
    {
        _db = db;
        _current = current;
    }

    public async Task<Result> Handle(TransferStudentCommand request, CancellationToken ct)
    {
        // بررسی وجود ثبت‌نام فعال در کلاس مبدأ
        var fromEnrollment = await _db.Enrollments
            .FirstOrDefaultAsync(e =>
                e.StudentId == request.StudentId &&
                e.ClassId == request.FromClassId &&
                e.Status == EnrollmentStatus.Approved, ct);

        if (fromEnrollment is null)
            return Result.Failure("ثبت‌نام فعالی در کلاس مبدأ یافت نشد.");

        // بررسی کلاس مقصد
        var toClass = await _db.Classes
            .FirstOrDefaultAsync(c => c.Id == request.ToClassId && c.Active, ct);

        if (toClass is null)
            return Result.Failure("کلاس مقصد معتبر نیست.");

        // بررسی ظرفیت کلاس مقصد
        var toCount = await _db.Enrollments
            .CountAsync(e => e.ClassId == request.ToClassId
                          && e.Status == EnrollmentStatus.Approved, ct);

        if (toClass.Capacity > 0 && toCount >= toClass.Capacity)
            return Result.Failure("ظرفیت کلاس مقصد پر است.");

        // بررسی عدم وجود ثبت‌نام تکراری در کلاس مقصد
        var exists = await _db.Enrollments.AnyAsync(e =>
            e.StudentId == request.StudentId &&
            e.ClassId == request.ToClassId &&
            e.Status == EnrollmentStatus.Approved, ct);

        if (exists)
            return Result.Failure("شاگرد قبلاً در کلاس مقصد ثبت‌نام شده است.");

        // لغو ثبت‌نام مبدأ
        fromEnrollment.Status = EnrollmentStatus.Canceled;

        // ایجاد ثبت‌نام جدید
        _db.Enrollments.Add(new Enrollment
        {
            StudentId = request.StudentId,
            ClassId = request.ToClassId,
            Status = EnrollmentStatus.Approved,
            RequestedAt = DateTime.UtcNow,
            ApprovedAt = DateTime.UtcNow,
            ApprovedBy = _current.UserId
        });

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}