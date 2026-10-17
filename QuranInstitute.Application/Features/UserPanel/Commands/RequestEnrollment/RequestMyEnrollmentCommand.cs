using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.UserPanel.Commands.RequestEnrollment;

public class RequestMyEnrollmentCommand : IRequest<Result>
{
    public Guid StudentId { get; set; }
    public Guid ClassId { get; set; }
}