using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Admin.Users.Commands.ActivateUser;

public class ActivateUserCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public ActivateUserCommand(Guid userId) => UserId = userId;
}