using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Enrollments.Queries.GetEnrollmentsList;

public class GetEnrollmentsListQueryHandler
    : IRequestHandler<GetEnrollmentsListQuery, PagedList<EnrollmentListItemDto>>
{
    private readonly IApplicationDbContext _db;

    public GetEnrollmentsListQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<PagedList<EnrollmentListItemDto>> Handle(
        GetEnrollmentsListQuery request, CancellationToken ct)
    {
        var q = _db.Enrollments
            .AsNoTracking()
            .Include(e => e.Student)
            .Include(e => e.Class)
            .AsQueryable();

        if (request.ClassId.HasValue)
            q = q.Where(e => e.ClassId == request.ClassId.Value);

        if (request.Status.HasValue)
            q = q.Where(e => e.Status == request.Status.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            q = q.Where(e =>
                e.Student.Name.Contains(s) ||
                e.Student.ParentMobile.Contains(s) ||
                e.Class.Name.Contains(s));
        }

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderByDescending(e => e.RequestedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new EnrollmentListItemDto
            {
                Id = e.Id,
                StudentId = e.StudentId,
                StudentName = e.Student.Name,
                ParentMobile = e.Student.ParentMobile,
                ClassId = e.ClassId,
                ClassName = e.Class.Name,
                Status = e.Status,
                RequestedAt = e.RequestedAt,
                ApprovedAt = e.ApprovedAt,
                Note = e.Note
            })
            .ToListAsync(ct);

        return new PagedList<EnrollmentListItemDto>
        {
            Items = items,
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}