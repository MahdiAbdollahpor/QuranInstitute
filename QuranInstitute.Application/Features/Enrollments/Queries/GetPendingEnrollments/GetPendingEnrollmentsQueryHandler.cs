using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Features.Enrollments.Queries.GetEnrollmentsList;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Enrollments.Queries.GetPendingEnrollments;

public class GetPendingEnrollmentsQueryHandler
    : IRequestHandler<GetPendingEnrollmentsQuery, List<EnrollmentListItemDto>>
{
    private readonly IApplicationDbContext _db;

    public GetPendingEnrollmentsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<List<EnrollmentListItemDto>> Handle(
        GetPendingEnrollmentsQuery request, CancellationToken ct)
    {
        return await _db.Enrollments
            .AsNoTracking()
            .Where(e => e.Status == EnrollmentStatus.Pending)
            .OrderBy(e => e.RequestedAt)
            .Select(e => new EnrollmentListItemDto
            {
                Id = e.Id,
                StudentId = e.StudentId,
                StudentName = e.Student.Name,
                ParentMobile = e.Student.ParentMobile,
                ClassId = e.ClassId,
                ClassName = e.Class.Name,
                Status = e.Status,
                RequestedAt = e.RequestedAt
            })
            .ToListAsync(ct);
    }
}