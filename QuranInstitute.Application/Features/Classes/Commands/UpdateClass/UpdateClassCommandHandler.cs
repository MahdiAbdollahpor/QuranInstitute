using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Classes.Commands.UpdateClass;

public class UpdateClassCommandHandler
    : IRequestHandler<UpdateClassCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public UpdateClassCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateClassCommand request, CancellationToken ct)
    {
        var entity = await _db.Classes
            .FirstOrDefaultAsync(c => c.Id == request.Id, ct);

        if (entity is null)
            return Result.Failure("کلاس یافت نشد.");

        // بررسی تکراری نبودن نام
        var dup = await _db.Classes.AnyAsync(c =>
            c.Id != request.Id &&
            c.Name == request.Name.Trim() &&
            !c.IsDeleted, ct);

        if (dup)
            return Result.Failure("کلاسی با این نام قبلاً ثبت شده است.");

        entity.Name = request.Name.Trim();
        entity.TeacherName = request.TeacherName.Trim();
        entity.ClassTime = request.ClassTime?.Trim();
        entity.Days = request.Days;
        entity.Type = request.Type;
        entity.DefaultOnlineLink = request.DefaultOnlineLink?.Trim();
        entity.Capacity = request.Capacity;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}