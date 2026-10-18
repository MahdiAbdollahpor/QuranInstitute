using Microsoft.AspNetCore.Identity;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Domain.Entities.Users;

namespace QuranInstitute.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<AppUser> _userMgr;
    private readonly RoleManager<AppRole> _roleMgr;

    public IdentityService(
        UserManager<AppUser> userMgr,
        RoleManager<AppRole> roleMgr)
    {
        _userMgr = userMgr;
        _roleMgr = roleMgr;
    }

    public async Task<(bool Success, Guid? UserId, string? Error)> CreateUserAsync(
        string mobile,
        string fullName,
        string password,
        string roleName,
        CancellationToken ct = default)
    {
        if (!await _roleMgr.RoleExistsAsync(roleName))
            await _roleMgr.CreateAsync(new AppRole(roleName));

        var user = new AppUser
        {
            UserName = mobile,
            PhoneNumber = mobile,
            FullName = fullName,
            PhoneNumberConfirmed = true,
            IsActive = true
        };

        var result = await _userMgr.CreateAsync(user, password);
        if (!result.Succeeded)
            return (false, null, string.Join("، ", result.Errors.Select(e => e.Description)));

        await _userMgr.AddToRoleAsync(user, roleName);
        return (true, user.Id, null);
    }

    public async Task<bool> UserExistsAsync(string mobile, CancellationToken ct = default)
    {
        return await _userMgr.FindByNameAsync(mobile) is not null;
    }

    public async Task<(bool Success, string? Error)> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken ct = default)
    {
        var user = await _userMgr.FindByIdAsync(userId.ToString());
        if (user is null)
            return (false, "کاربر یافت نشد.");

        var result = await _userMgr.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
        {
            var msg = result.Errors.Any(e => e.Code == "PasswordMismatch")
                ? "رمز فعلی اشتباه است."
                : string.Join("، ", result.Errors.Select(e => e.Description));
            return (false, msg);
        }

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> ResetPasswordByAdminAsync(
        Guid userId,
        string newPassword,
        CancellationToken ct = default)
    {
        var user = await _userMgr.FindByIdAsync(userId.ToString());
        if (user is null)
            return (false, "کاربر یافت نشد.");

        var token = await _userMgr.GeneratePasswordResetTokenAsync(user);
        var result = await _userMgr.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded)
            return (false, string.Join("، ", result.Errors.Select(e => e.Description)));

        return (true, null);
    }
}