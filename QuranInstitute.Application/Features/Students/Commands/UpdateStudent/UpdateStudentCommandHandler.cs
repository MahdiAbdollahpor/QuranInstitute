using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Entities.Education;
using QuranInstitute.Domain.Enums;
using QuranInstitute.Shared.Helpers;

namespace QuranInstitute.Application.Features.Students.Commands.UpdateStudent;

public class UpdateStudentCommandHandler
    : IRequestHandler<UpdateStudentCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public UpdateStudentCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateStudentCommand request, CancellationToken ct)
    {
        var student = await _db.Students
            .FirstOrDefaultAsync(s => s.Id == request.Id, ct);

        if (student is null)
            return Result.Failure("شاگرد یافت نشد.");

        var mobile = NormalizeMobile(request.ParentMobile);

        // بررسی تکراری نبودن موبایل
        var dup = await _db.Students.AnyAsync(s =>
            s.Id != request.Id && s.ParentMobile == mobile && !s.IsDeleted, ct);
        if (dup)
            return Result.Failure("این شماره موبایل قبلاً برای شاگرد دیگری ثبت شده است.");

        student.Name = request.Name.Trim();
        student.ParentName = request.ParentName.Trim();
        student.ParentMobile = mobile;
        student.MonthlyFee = request.MonthlyFee;
        student.StartDate = request.StartDate;
        student.Note = request.Note?.Trim();

        // به‌روزرسانی ثبت‌نام کلاس
        var currentEnrollments = await _db.Enrollments
            .Where(e => e.StudentId == student.Id
                     && e.Status == EnrollmentStatus.Approved)
            .ToListAsync(ct);

        if (request.ClassId.HasValue)
        {
            var alreadyIn = currentEnrollments.Any(e => e.ClassId == request.ClassId.Value);

            if (!alreadyIn)
            {
                // لغو ثبت‌نام‌های قبلی
                foreach (var old in currentEnrollments)
                    old.Status = EnrollmentStatus.Canceled;

                // ثبت‌نام جدید
                _db.Enrollments.Add(new Enrollment
                {
                    StudentId = student.Id,
                    ClassId = request.ClassId.Value,
                    Status = EnrollmentStatus.Approved,
                    RequestedAt = DateTime.UtcNow,
                    ApprovedAt = DateTime.UtcNow
                });
            }
        }
        else
        {
            // حذف کلاس‌ها
            foreach (var e in currentEnrollments)
                e.Status = EnrollmentStatus.Canceled;
        }

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static string NormalizeMobile(string input)
    {
        var n = input.ConvertToEn().Trim().Replace(" ", "").Replace("-", "");
        if (n.StartsWith("+98")) n = "0" + n.Substring(3);
        if (n.StartsWith("0098")) n = "0" + n.Substring(4);
        if (n.StartsWith("98") && n.Length == 12) n = "0" + n.Substring(2);
        return n;
    }
}