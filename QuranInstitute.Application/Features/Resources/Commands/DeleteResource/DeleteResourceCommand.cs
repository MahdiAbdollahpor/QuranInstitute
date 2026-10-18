using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Resources.Commands.DeleteResource;

public class DeleteResourceCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    public DeleteResourceCommand(Guid id) => Id = id;
}