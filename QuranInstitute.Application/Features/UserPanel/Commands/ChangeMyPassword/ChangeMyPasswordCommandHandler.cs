using MediatR;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.UserPanel.Commands.ChangeMyPassword;

public class ChangeMyPasswordCommandHandler
    : IRequestHandler<ChangeMyPasswordCommand, Result>
{
    private readonly IIdentityService _identity;
    private readonly ICurrentUserService _current;

    public ChangeMyPasswordCommandHandler(
        IIdentityService identity,
        ICurrentUserService current)
    {
        _identity = identity;
        _current = current;
    }

    public async Task<Result> Handle(
        ChangeMyPasswordCommand request, CancellationToken ct)
    {
        var userId = _current.UserId;
        if (!userId.HasValue)
            return Result.Failure("کاربر شناسایی نشد.");

        var (success, error) = await _identity.ChangePasswordAsync(
            userId.Value,
            request.CurrentPassword,
            request.NewPassword,
            ct);

        if (!success)
            return Result.Failure(error ?? "خطا در تغییر رمز.");

        return Result.Success();
    }
}