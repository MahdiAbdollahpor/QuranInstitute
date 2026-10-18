using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Resources.Queries.GetResourcesByClass;

public class GetResourcesByClassQueryHandler
    : IRequestHandler<GetResourcesByClassQuery, Result<List<ResourceListItemDto>>>
{
    private readonly IApplicationDbContext _db;
    private readonly IFileStorageService _storage;

    public GetResourcesByClassQueryHandler(
        IApplicationDbContext db,
        IFileStorageService storage)
    {
        _db = db;
        _storage = storage;
    }

    public async Task<Result<List<ResourceListItemDto>>> Handle(
        GetResourcesByClassQuery request, CancellationToken ct)
    {
        var q = _db.ClassResources
            .AsNoTracking()
            .Where(r => r.ClassId == request.ClassId);

        if (request.SessionId.HasValue)
            q = q.Where(r => r.SessionId == request.SessionId.Value);

        var raw = await q
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new
            {
                r.Id,
                r.ClassId,
                ClassName = r.Class.Name,
                r.SessionId,
                SessionNumber = r.Session != null ? (int?)r.Session.SessionNumber : null,
                r.Type,
                r.Title,
                r.Description,
                r.FileUrl,
                r.FileSize,
                r.MimeType,
                r.EnrolledOnly,
                r.CreatedAt
            })
            .ToListAsync(ct);

        var items = raw.Select(r => new ResourceListItemDto
        {
            Id = r.Id,
            ClassId = r.ClassId,
            ClassName = r.ClassName,
            SessionId = r.SessionId,
            SessionNumber = r.SessionNumber,
            Type = r.Type,
            Title = r.Title,
            Description = r.Description,
            FileUrl = r.FileUrl,
            PublicUrl = _storage.GetPublicUrl(r.FileUrl),
            FileSize = r.FileSize,
            MimeType = r.MimeType,
            EnrolledOnly = r.EnrolledOnly,
            CreatedAt = r.CreatedAt
        }).ToList();

        return Result<List<ResourceListItemDto>>.Success(items);
    }
}