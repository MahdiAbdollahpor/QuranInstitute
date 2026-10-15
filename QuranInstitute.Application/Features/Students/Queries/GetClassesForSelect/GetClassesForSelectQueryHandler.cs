using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;

namespace QuranInstitute.Application.Features.Students.Queries.GetClassesForSelect;

public class GetClassesForSelectQueryHandler
    : IRequestHandler<GetClassesForSelectQuery, List<ClassSelectItemDto>>
{
    private readonly IApplicationDbContext _db;

    public GetClassesForSelectQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<List<ClassSelectItemDto>> Handle(
        GetClassesForSelectQuery request, CancellationToken ct)
    {
        return await _db.Classes
            .AsNoTracking()
            .Where(c => c.Active)
            .OrderBy(c => c.Name)
            .Select(c => new ClassSelectItemDto
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync(ct);
    }
}