namespace QuranInstitute.Application.Common.Interfaces;

public interface IIdentityService
{
    // ─── ساخت کاربر جدید ───
    Task<(bool Success, Guid? UserId, string? Error)> CreateUserAsync(
        string mobile,
        string fullName,
        string password,
        string roleName,
        CancellationToken ct = default);

    // ─── بررسی وجود کاربر ───
    Task<bool> UserExistsAsync(string mobile, CancellationToken ct = default);

    // ─── تغییر رمز توسط خود کاربر (نیاز به رمز فعلی) ───
    Task<(bool Success, string? Error)> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken ct = default);

    // ─── ریست رمز توسط ادمین (بدون رمز فعلی) ───
    Task<(bool Success, string? Error)> ResetPasswordByAdminAsync(
        Guid userId,
        string newPassword,
        CancellationToken ct = default);
}