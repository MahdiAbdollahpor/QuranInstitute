using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Enrollments.Commands.CancelEnrollment;

public class CancelEnrollmentCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    public CancelEnrollmentCommand(Guid id) => Id = id;
}