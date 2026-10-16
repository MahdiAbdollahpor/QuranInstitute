using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Enrollments.Commands.ApproveEnrollment;

public class ApproveEnrollmentCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    public ApproveEnrollmentCommand(Guid id) => Id = id;
}