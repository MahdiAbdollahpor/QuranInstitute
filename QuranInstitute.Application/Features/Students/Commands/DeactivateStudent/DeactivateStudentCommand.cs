using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Students.Commands.DeactivateStudent;

public class DeactivateStudentCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    public DeactivateStudentCommand(Guid id) => Id = id;
}