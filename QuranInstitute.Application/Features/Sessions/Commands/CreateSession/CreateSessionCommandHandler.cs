using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Entities.Education;

namespace QuranInstitute.Application.Features.Sessions.Commands.CreateSession;

public class CreateSessionCommandHandler
    : IRequestHandler<CreateSessionCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _db;

    public CreateSessionCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(
        CreateSessionCommand request, CancellationToken ct)
    {
        var clsExists = await _db.Classes
            .AnyAsync(c => c.Id == request.ClassId && !c.IsDeleted, ct);

        if (!clsExists)
            return Result<Guid>.Failure("کلاس یافت نشد.");

        var dup = await _db.ClassSessions
            .AnyAsync(s => s.ClassId == request.ClassId
                        && s.SessionNumber == request.SessionNumber, ct);

        if (dup)
            return Result<Guid>.Failure(
                $"جلسه شماره {request.SessionNumber} قبلاً در این کلاس ثبت شده است.");

        var session = new ClassSession
        {
            ClassId = request.ClassId,
            SessionNumber = request.SessionNumber,
            Date = request.Date.Date,
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            IsOnline = request.IsOnline,
            OnlineLink = request.OnlineLink?.Trim(),
            RecordingUrl = request.RecordingUrl?.Trim()
        };

        _db.ClassSessions.Add(session);
        await _db.SaveChangesAsync(ct);

        return Result<Guid>.Success(session.Id);
    }
}