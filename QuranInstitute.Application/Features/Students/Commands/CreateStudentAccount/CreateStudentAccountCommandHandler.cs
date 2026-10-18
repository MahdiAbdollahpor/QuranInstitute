using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Constants;
using QuranInstitute.Shared.Helpers;

namespace QuranInstitute.Application.Features.Students.Commands.CreateStudentAccount;

public class CreateStudentAccountCommandHandler
    : IRequestHandler<CreateStudentAccountCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly IIdentityService _identity;

    public CreateStudentAccountCommandHandler(
        IApplicationDbContext db,
        IIdentityService identity)
    {
        _db = db;
        _identity = identity;
    }

    public async Task<Result> Handle(
        CreateStudentAccountCommand request, CancellationToken ct)
    {
        var student = await _db.Students
            .FirstOrDefaultAsync(s => s.Id == request.StudentId, ct);

        if (student is null)
            return Result.Failure("شاگرد یافت نشد.");

        if (student.UserId.HasValue)
            return Result.Failure("این شاگرد قبلاً حساب کاربری دارد.");

        var mobile = student.ParentMobile.ConvertToEn();

        if (await _identity.UserExistsAsync(mobile, ct))
            return Result.Failure("کاربری با این شماره موبایل قبلاً ثبت‌نام کرده است.");

        // ★ همیشه Parent
        var roleName = Roles.Parent;
        var fullName = $"{student.ParentName} (ولی {student.Name})";

        var (success, userId, error) = await _identity.CreateUserAsync(
            mobile, fullName, request.Password, roleName, ct);

        if (!success)
            return Result.Failure(error ?? "خطا در ساخت حساب.");

        student.UserId = userId;
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}