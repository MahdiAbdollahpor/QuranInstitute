using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Admin.Users.Commands.DeactivateUser;

public class DeactivateUserCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public DeactivateUserCommand(Guid userId) => UserId = userId;
}