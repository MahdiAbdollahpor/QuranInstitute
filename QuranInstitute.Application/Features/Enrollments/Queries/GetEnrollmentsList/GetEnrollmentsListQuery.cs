using MediatR;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Enrollments.Queries.GetEnrollmentsList;

public class GetEnrollmentsListQuery : IRequest<PagedList<EnrollmentListItemDto>>
{
    public string? Search { get; set; }
    public Guid? ClassId { get; set; }
    public EnrollmentStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}