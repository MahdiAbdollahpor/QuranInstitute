using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Classes.Queries.GetClassById;

public class GetClassByIdQueryHandler
    : IRequestHandler<GetClassByIdQuery, Result<ClassDetailDto>>
{
    private readonly IApplicationDbContext _db;

    public GetClassByIdQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<ClassDetailDto>> Handle(
        GetClassByIdQuery request, CancellationToken ct)
    {
        // ─── مرحله ۱: گرفته داده خام ───
        var raw = await _db.Classes
            .AsNoTracking()
            .Where(c => c.Id == request.Id)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.TeacherName,
                c.ClassTime,
                c.DaysCsv,
                c.Type,
                c.DefaultOnlineLink,
                c.Capacity,
                c.Active
            })
            .FirstOrDefaultAsync(ct);

        if (raw is null)
            return Result<ClassDetailDto>.Failure("کلاس یافت نشد.");

        // ─── مرحله ۲: تبدیل در حافظه ───
        var dto = new ClassDetailDto
        {
            Id = raw.Id,
            Name = raw.Name,
            TeacherName = raw.TeacherName,
            ClassTime = raw.ClassTime,
            Days = string.IsNullOrWhiteSpace(raw.DaysCsv)
                ? Array.Empty<int>()
                : raw.DaysCsv.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(int.Parse)
                            .ToArray(),
            Type = raw.Type,
            DefaultOnlineLink = raw.DefaultOnlineLink,
            Capacity = raw.Capacity,
            Active = raw.Active
        };

        return Result<ClassDetailDto>.Success(dto);
    }
}