using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Students.Queries.GetStudentsList;

public class GetStudentsListQueryHandler
    : IRequestHandler<GetStudentsListQuery, PagedList<StudentListItemDto>>
{
    private readonly IApplicationDbContext _db;

    public GetStudentsListQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<PagedList<StudentListItemDto>> Handle(
        GetStudentsListQuery request, CancellationToken ct)
    {
        var q = _db.Students.AsNoTracking().AsQueryable();

        if (request.ActiveOnly == true)
            q = q.Where(s => s.Active);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            q = q.Where(s =>
                s.Name.Contains(search) ||
                s.ParentName.Contains(search) ||
                s.ParentMobile.Contains(search));
        }

        // فیلتر کلاس: از روی Enrollments تأییدشده
        if (request.ClassId.HasValue)
        {
            q = q.Where(s => s.Enrollments.Any(e =>
                e.ClassId == request.ClassId.Value &&
                e.Status == Domain.Enums.EnrollmentStatus.Approved));
        }

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderByDescending(s => s.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new StudentListItemDto
            {
                Id = s.Id,
                Name = s.Name,
                ParentName = s.ParentName,
                ParentMobile = s.ParentMobile,
                ClassId = s.Enrollments
                    .Where(e => e.Status == Domain.Enums.EnrollmentStatus.Approved)
                    .Select(e => (Guid?)e.ClassId)
                    .FirstOrDefault(),
                ClassName = s.Enrollments
                    .Where(e => e.Status == Domain.Enums.EnrollmentStatus.Approved)
                    .Select(e => e.Class.Name)
                    .FirstOrDefault() ?? "—",
                MonthlyFee = s.MonthlyFee,
                StartDate = s.StartDate,
                Active = s.Active,
                UserId = s.UserId
            })
            .ToListAsync(ct);

        return new PagedList<StudentListItemDto>
        {
            Items = items,
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}