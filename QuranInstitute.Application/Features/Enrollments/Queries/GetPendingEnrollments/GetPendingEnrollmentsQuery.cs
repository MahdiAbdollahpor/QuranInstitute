using MediatR;
using QuranInstitute.Application.Features.Enrollments.Queries.GetEnrollmentsList;

namespace QuranInstitute.Application.Features.Enrollments.Queries.GetPendingEnrollments;

public class GetPendingEnrollmentsQuery : IRequest<List<EnrollmentListItemDto>>
{
}