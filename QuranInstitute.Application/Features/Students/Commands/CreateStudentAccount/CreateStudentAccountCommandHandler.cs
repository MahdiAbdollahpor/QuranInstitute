using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Constants;
using QuranInstitute.Domain.Entities.Users;
using QuranInstitute.Shared.Helpers;

namespace QuranInstitute.Application.Features.Students.Commands.CreateStudentAccount;

public class CreateStudentAccountCommandHandler
    : IRequestHandler<CreateStudentAccountCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly UserManager<AppUser> _userMgr;
    private readonly RoleManager<AppRole> _roleMgr;

    public CreateStudentAccountCommandHandler(
        IApplicationDbContext db,
        UserManager<AppUser> userMgr,
        RoleManager<AppRole> roleMgr)
    {
        _db = db;
        _userMgr = userMgr;
        _roleMgr = roleMgr;
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

        // بررسی نبودن کاربر با این موبایل
        var mobile = student.ParentMobile.ConvertToEn();
        var existing = await _userMgr.FindByNameAsync(mobile);
        if (existing is not null)
            return Result.Failure("کاربری با این شماره موبایل قبلاً ثبت‌نام کرده است.");

        var roleName = request.Role == "Student" ? Roles.Student : Roles.Parent;

        if (!await _roleMgr.RoleExistsAsync(roleName))
            await _roleMgr.CreateAsync(new AppRole(roleName));

        var user = new AppUser
        {
            UserName = mobile,
            PhoneNumber = mobile,
            FullName = roleName == Roles.Parent
                ? $"{student.ParentName} (ولی {student.Name})"
                : student.Name,
            PhoneNumberConfirmed = true,
            IsActive = true
        };

        var result = await _userMgr.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return Result.Failure(string.Join("، ", result.Errors.Select(e => e.Description)));

        await _userMgr.AddToRoleAsync(user, roleName);

        student.UserId = user.Id;
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}