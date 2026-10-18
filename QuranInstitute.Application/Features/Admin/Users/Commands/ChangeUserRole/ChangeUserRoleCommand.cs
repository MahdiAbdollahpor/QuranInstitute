using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Admin.Users.Commands.ChangeUserRole;

public class ChangeUserRoleCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public string NewRole { get; set; } = string.Empty;

    public ChangeUserRoleCommand(Guid userId, string newRole)
    {
        UserId = userId;
        NewRole = newRole;
    }
}