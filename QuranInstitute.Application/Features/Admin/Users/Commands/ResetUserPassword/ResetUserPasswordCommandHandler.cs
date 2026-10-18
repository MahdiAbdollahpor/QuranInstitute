using MediatR;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Admin.Users.Commands.ResetUserPassword;

public class ResetUserPasswordCommandHandler
    : IRequestHandler<ResetUserPasswordCommand, Result>
{
    private readonly IIdentityService _identity;

    public ResetUserPasswordCommandHandler(IIdentityService identity)
        => _identity = identity;

    public async Task<Result> Handle(
        ResetUserPasswordCommand request, CancellationToken ct)
    {
        var (success, error) = await _identity.ResetPasswordByAdminAsync(
            request.UserId, request.NewPassword, ct);

        if (!success)
            return Result.Failure(error ?? "خطا در تغییر رمز.");

        return Result.Success();
    }
}