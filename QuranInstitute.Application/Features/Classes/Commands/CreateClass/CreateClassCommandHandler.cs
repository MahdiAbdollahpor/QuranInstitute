using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Entities.Education;

namespace QuranInstitute.Application.Features.Classes.Commands.CreateClass;

public class CreateClassCommandHandler
    : IRequestHandler<CreateClassCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _db;

    public CreateClassCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(
        CreateClassCommand request, CancellationToken ct)
    {
        // بررسی تکراری نبودن نام
        var exists = await _db.Classes
            .AnyAsync(c => c.Name == request.Name.Trim() && !c.IsDeleted, ct);

        if (exists)
            return Result<Guid>.Failure("کلاسی با این نام قبلاً ثبت شده است.");

        var entity = new Class
        {
            Name = request.Name.Trim(),
            TeacherName = request.TeacherName.Trim(),
            ClassTime = request.ClassTime?.Trim(),
            Days = request.Days,        // Setter آن DaysCsv را پر می‌کند
            Type = request.Type,
            DefaultOnlineLink = request.DefaultOnlineLink?.Trim(),
            Capacity = request.Capacity,
            Active = true
        };

        _db.Classes.Add(entity);
        await _db.SaveChangesAsync(ct);

        return Result<Guid>.Success(entity.Id);
    }
}