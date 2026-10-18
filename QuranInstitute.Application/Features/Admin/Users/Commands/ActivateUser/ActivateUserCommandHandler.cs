using MediatR;
using Microsoft.AspNetCore.Identity;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Entities.Users;

namespace QuranInstitute.Application.Features.Admin.Users.Commands.ActivateUser;

public class ActivateUserCommandHandler
    : IRequestHandler<ActivateUserCommand, Result>
{
    private readonly UserManager<AppUser> _userMgr;

    public ActivateUserCommandHandler(UserManager<AppUser> userMgr)
        => _userMgr = userMgr;

    public async Task<Result> Handle(
        ActivateUserCommand request, CancellationToken ct)
    {
        var user = await _userMgr.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            return Result.Failure("کاربر یافت نشد.");

        if (user.IsActive)
            return Result.Failure("این کاربر قبلاً فعال است.");

        user.IsActive = true;
        await _userMgr.UpdateAsync(user);

        return Result.Success();
    }
}