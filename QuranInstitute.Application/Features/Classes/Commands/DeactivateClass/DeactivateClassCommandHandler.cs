using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Classes.Commands.DeactivateClass;

public class DeactivateClassCommandHandler
    : IRequestHandler<DeactivateClassCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public DeactivateClassCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(DeactivateClassCommand request, CancellationToken ct)
    {
        var entity = await _db.Classes
            .FirstOrDefaultAsync(c => c.Id == request.Id, ct);

        if (entity is null)
            return Result.Failure("کلاس یافت نشد.");

        // اگر شاگرد فعال دارد، هشدار بده
        var activeCount = await _db.Enrollments
            .CountAsync(e => e.ClassId == request.Id
                          && e.Status == EnrollmentStatus.Approved, ct);

        if (activeCount > 0)
        {
            // به‌جای حذف، فقط غیرفعال می‌کنیم
            entity.Active = false;
        }
        else
        {
            entity.Active = false;
        }

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}