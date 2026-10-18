using MediatR;
using Microsoft.AspNetCore.Identity;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Entities.Users;

namespace QuranInstitute.Application.Features.Admin.Users.Commands.DeactivateUser;

public class DeactivateUserCommandHandler
    : IRequestHandler<DeactivateUserCommand, Result>
{
    private readonly UserManager<AppUser> _userMgr;
    private readonly ICurrentUserService _current;

    public DeactivateUserCommandHandler(
        UserManager<AppUser> userMgr,
        ICurrentUserService current)
    {
        _userMgr = userMgr;
        _current = current;
    }

    public async Task<Result> Handle(
        DeactivateUserCommand request, CancellationToken ct)
    {
        // جلوگیری از غیرفعال کردن خود
        if (_current.UserId == request.UserId)
            return Result.Failure("نمی‌توانید حساب خودتان را غیرفعال کنید.");

        var user = await _userMgr.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            return Result.Failure("کاربر یافت نشد.");

        if (!user.IsActive)
            return Result.Failure("این کاربر قبلاً غیرفعال شده است.");

        user.IsActive = false;
        await _userMgr.UpdateAsync(user);

        // به‌روزرسانی امنیتی — خروج از همه session ها
        await _userMgr.UpdateSecurityStampAsync(user);

        return Result.Success();
    }
}