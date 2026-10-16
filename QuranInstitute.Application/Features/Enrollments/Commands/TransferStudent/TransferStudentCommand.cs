using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Enrollments.Commands.TransferStudent;

public class TransferStudentCommand : IRequest<Result>
{
    public Guid StudentId { get; set; }
    public Guid FromClassId { get; set; }
    public Guid ToClassId { get; set; }
}