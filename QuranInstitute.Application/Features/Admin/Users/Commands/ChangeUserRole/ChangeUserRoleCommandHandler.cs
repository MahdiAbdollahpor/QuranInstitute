using MediatR;
using Microsoft.AspNetCore.Identity;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Entities.Users;

namespace QuranInstitute.Application.Features.Admin.Users.Commands.ChangeUserRole;

public class ChangeUserRoleCommandHandler
    : IRequestHandler<ChangeUserRoleCommand, Result>
{
    private readonly UserManager<AppUser> _userMgr;
    private readonly RoleManager<AppRole> _roleMgr;
    private readonly ICurrentUserService _current;

    public ChangeUserRoleCommandHandler(
        UserManager<AppUser> userMgr,
        RoleManager<AppRole> roleMgr,
        ICurrentUserService current)
    {
        _userMgr = userMgr;
        _roleMgr = roleMgr;
        _current = current;
    }

    public async Task<Result> Handle(
        ChangeUserRoleCommand request, CancellationToken ct)
    {
        if (_current.UserId == request.UserId)
            return Result.Failure("نمی‌توانید نقش خودتان را تغییر دهید.");

        var user = await _userMgr.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            return Result.Failure("کاربر یافت نشد.");

        // بررسی وجود نقش
        if (!await _roleMgr.RoleExistsAsync(request.NewRole))
            await _roleMgr.CreateAsync(new AppRole(request.NewRole));

        // نقش فعلی
        var currentRoles = await _userMgr.GetRolesAsync(user);

        if (currentRoles.Count == 1 && currentRoles[0] == request.NewRole)
            return Result.Failure("کاربر از قبل همین نقش را دارد.");

        // اگر Admin است و آخرین ادمین فعال، جلوگیری از تغییر
        if (currentRoles.Contains("Admin"))
        {
            var admins = await _userMgr.GetUsersInRoleAsync("Admin");
            if (admins.Count(a => a.IsActive) <= 1)
                return Result.Failure("نمی‌توانید نقش آخرین ادمین فعال را تغییر دهید.");
        }

        // حذف همه نقش‌های فعلی
        if (currentRoles.Any())
            await _userMgr.RemoveFromRolesAsync(user, currentRoles);

        // افزودن نقش جدید
        await _userMgr.AddToRoleAsync(user, request.NewRole);

        // به‌روزرسانی امنیتی (خروج از session)
        await _userMgr.UpdateSecurityStampAsync(user);

        return Result.Success();
    }
}