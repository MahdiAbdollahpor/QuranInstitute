using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Sessions.Commands.DeleteSession;

public class DeleteSessionCommandHandler
    : IRequestHandler<DeleteSessionCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public DeleteSessionCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteSessionCommand request, CancellationToken ct)
    {
        var session = await _db.ClassSessions
            .FirstOrDefaultAsync(s => s.Id == request.Id, ct);

        if (session is null)
            return Result.Failure("جلسه یافت نشد.");

        // فایل‌های متصل به این جلسه، SessionId آن‌ها null می‌شود (SetNull در Config)
        // اگر می‌خواهی آن‌ها هم حذف شوند، این‌جا دستی حذف کن
        _db.ClassSessions.Remove(session);
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}