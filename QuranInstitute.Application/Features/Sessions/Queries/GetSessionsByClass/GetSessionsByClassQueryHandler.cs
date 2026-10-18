using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Sessions.Queries.GetSessionsByClass;

public class GetSessionsByClassQueryHandler
    : IRequestHandler<GetSessionsByClassQuery, Result<SessionsListResult>>
{
    private readonly IApplicationDbContext _db;

    public GetSessionsByClassQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<SessionsListResult>> Handle(
        GetSessionsByClassQuery request, CancellationToken ct)
    {
        var cls = await _db.Classes
            .AsNoTracking()
            .Where(c => c.Id == request.ClassId)
            .Select(c => new { c.Id, c.Name, c.TeacherName })
            .FirstOrDefaultAsync(ct);

        if (cls is null)
            return Result<SessionsListResult>.Failure("کلاس یافت نشد.");

        var sessions = await _db.ClassSessions
            .AsNoTracking()
            .Where(s => s.ClassId == request.ClassId)
            .OrderByDescending(s => s.SessionNumber)
            .Select(s => new SessionListItemDto
            {
                Id = s.Id,
                ClassId = s.ClassId,
                ClassName = s.Class.Name,
                SessionNumber = s.SessionNumber,
                Date = s.Date,
                Title = s.Title,
                Description = s.Description,
                IsOnline = s.IsOnline,
                OnlineLink = s.OnlineLink,
                RecordingUrl = s.RecordingUrl,
                ResourceCount = s.Resources.Count
            })
            .ToListAsync(ct);

        return Result<SessionsListResult>.Success(new SessionsListResult
        {
            ClassId = cls.Id,
            ClassName = cls.Name,
            TeacherName = cls.TeacherName,
            Sessions = sessions
        });
    }
}