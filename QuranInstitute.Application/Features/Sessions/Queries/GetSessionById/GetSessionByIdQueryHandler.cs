using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Sessions.Queries.GetSessionById;

public class GetSessionByIdQueryHandler
    : IRequestHandler<GetSessionByIdQuery, Result<SessionDetailDto>>
{
    private readonly IApplicationDbContext _db;

    public GetSessionByIdQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<SessionDetailDto>> Handle(
        GetSessionByIdQuery request, CancellationToken ct)
    {
        var session = await _db.ClassSessions
            .AsNoTracking()
            .Where(s => s.Id == request.Id)
            .Select(s => new SessionDetailDto
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
                RecordingUrl = s.RecordingUrl
            })
            .FirstOrDefaultAsync(ct);

        if (session is null)
            return Result<SessionDetailDto>.Failure("جلسه یافت نشد.");

        return Result<SessionDetailDto>.Success(session);
    }
}