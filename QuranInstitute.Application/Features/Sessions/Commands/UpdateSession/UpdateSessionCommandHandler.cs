using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Sessions.Commands.UpdateSession;

public class UpdateSessionCommandHandler
    : IRequestHandler<UpdateSessionCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public UpdateSessionCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateSessionCommand request, CancellationToken ct)
    {
        var session = await _db.ClassSessions
            .FirstOrDefaultAsync(s => s.Id == request.Id, ct);

        if (session is null)
            return Result.Failure("جلسه یافت نشد.");

        var dup = await _db.ClassSessions
            .AnyAsync(s => s.Id != request.Id
                        && s.ClassId == session.ClassId
                        && s.SessionNumber == request.SessionNumber, ct);

        if (dup)
            return Result.Failure(
                $"جلسه شماره {request.SessionNumber} قبلاً ثبت شده است.");

        session.SessionNumber = request.SessionNumber;
        session.Date = request.Date.Date;
        session.Title = request.Title.Trim();
        session.Description = request.Description?.Trim();
        session.IsOnline = request.IsOnline;
        session.OnlineLink = request.OnlineLink?.Trim();
        session.RecordingUrl = request.RecordingUrl?.Trim();

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}