using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Admin.Users.Commands.DeleteUser;

public class DeleteUserCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public DeleteUserCommand(Guid userId) => UserId = userId;
}