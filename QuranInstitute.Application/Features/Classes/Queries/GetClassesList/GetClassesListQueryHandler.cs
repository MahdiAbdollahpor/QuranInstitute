using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Classes.Queries.GetClassesList;

public class GetClassesListQueryHandler
    : IRequestHandler<GetClassesListQuery, PagedList<ClassListItemDto>>
{
    private readonly IApplicationDbContext _db;

    public GetClassesListQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<PagedList<ClassListItemDto>> Handle(
        GetClassesListQuery request, CancellationToken ct)
    {
        var q = _db.Classes.AsNoTracking().AsQueryable();

        if (request.ActiveOnly == true)
            q = q.Where(c => c.Active);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            q = q.Where(c =>
                c.Name.Contains(s) ||
                c.TeacherName.Contains(s));
        }

        var total = await q.CountAsync(ct);

        // ─── مرحله ۱: گرفتن داده خام از DB (قابل ترجمه به SQL) ───
        var raw = await q
            .OrderByDescending(c => c.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.TeacherName,
                c.ClassTime,
                c.DaysCsv,
                c.Type,
                c.Capacity,
                c.Active,
                StudentCount = c.Enrollments.Count(e => e.Status == EnrollmentStatus.Approved)
            })
            .ToListAsync(ct);

        // ─── مرحله ۲: تبدیل در حافظه (بعد از گرفتن داده) ───
        var items = raw.Select(c => new ClassListItemDto
        {
            Id = c.Id,
            Name = c.Name,
            TeacherName = c.TeacherName,
            ClassTime = c.ClassTime,
            Days = string.IsNullOrWhiteSpace(c.DaysCsv)
                ? Array.Empty<int>()
                : c.DaysCsv.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(int.Parse)
                          .ToArray(),
            Type = c.Type,
            Capacity = c.Capacity,
            StudentCount = c.StudentCount,
            Active = c.Active
        }).ToList();

        return new PagedList<ClassListItemDto>
        {
            Items = items,
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}