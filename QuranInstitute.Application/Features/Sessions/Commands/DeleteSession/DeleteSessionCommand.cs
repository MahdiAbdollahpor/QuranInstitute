using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Sessions.Commands.DeleteSession;

public class DeleteSessionCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    public DeleteSessionCommand(Guid id) => Id = id;
}