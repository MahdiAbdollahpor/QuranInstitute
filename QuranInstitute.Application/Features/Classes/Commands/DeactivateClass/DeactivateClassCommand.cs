using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Classes.Commands.DeactivateClass;

public class DeactivateClassCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    public DeactivateClassCommand(Guid id) => Id = id;
}