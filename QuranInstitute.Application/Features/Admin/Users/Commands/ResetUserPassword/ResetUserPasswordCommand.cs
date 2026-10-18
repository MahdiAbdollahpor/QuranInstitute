using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Admin.Users.Commands.ResetUserPassword;

public class ResetUserPasswordCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public string NewPassword { get; set; } = string.Empty;

    public ResetUserPasswordCommand(Guid userId, string newPassword)
    {
        UserId = userId;
        NewPassword = newPassword;
    }
}