using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Enrollments.Commands.RejectEnrollment;

public class RejectEnrollmentCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    public string? Reason { get; set; }

    public RejectEnrollmentCommand(Guid id, string? reason = null)
    {
        Id = id;
        Reason = reason;
    }
}