using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Entities.Education;
using QuranInstitute.Domain.Enums;
using QuranInstitute.Shared.Helpers;

namespace QuranInstitute.Application.Features.Students.Commands.CreateStudent;

public class CreateStudentCommandHandler
    : IRequestHandler<CreateStudentCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _db;

    public CreateStudentCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(
        CreateStudentCommand request, CancellationToken ct)
    {
        // نرمال‌سازی موبایل
        var mobile = NormalizeMobile(request.ParentMobile);

        // بررسی تکراری نبودن موبایل ولی (اختیاری - می‌توانی چک نکنی)
        var exists = await _db.Students.AnyAsync(s => s.ParentMobile == mobile && !s.IsDeleted, ct);
        if (exists)
            return Result<Guid>.Failure("این شماره موبایل قبلاً برای شاگرد دیگری ثبت شده است.");

        var student = new Student
        {
            Name = request.Name.Trim(),
            ParentName = request.ParentName.Trim(),
            ParentMobile = mobile,
            MonthlyFee = request.MonthlyFee,
            StartDate = request.StartDate,
            Note = request.Note?.Trim(),
            Active = true
        };

        _db.Students.Add(student);

        // اگر کلاس انتخاب شده، ثبت‌نام تأییدشده بساز
        if (request.ClassId.HasValue)
        {
            var classExists = await _db.Classes
                .AnyAsync(c => c.Id == request.ClassId.Value && c.Active, ct);

            if (!classExists)
                return Result<Guid>.Failure("کلاس انتخاب‌شده معتبر نیست.");

            _db.Enrollments.Add(new Enrollment
            {
                StudentId = student.Id,
                ClassId = request.ClassId.Value,
                Status = EnrollmentStatus.Approved,
                RequestedAt = DateTime.UtcNow,
                ApprovedAt = DateTime.UtcNow
            });
        }

        await _db.SaveChangesAsync(ct);

        return Result<Guid>.Success(student.Id);
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