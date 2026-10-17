using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.UserPanel.Queries.GetMyAttendance;

public class GetMyAttendanceQueryHandler
    : IRequestHandler<GetMyAttendanceQuery, Result<MyAttendanceDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _current;

    public GetMyAttendanceQueryHandler(
        IApplicationDbContext db,
        ICurrentUserService current)
    {
        _db = db;
        _current = current;
    }

    public async Task<Result<MyAttendanceDto>> Handle(
        GetMyAttendanceQuery request, CancellationToken ct)
    {
        var userId = _current.UserId;
        if (!userId.HasValue)
            return Result<MyAttendanceDto>.Failure("کاربر شناسایی نشد.");

        var student = await _db.Students
            .AsNoTracking()
            .Where(s => s.Id == request.StudentId && s.UserId == userId.Value)
            .Select(s => new { s.Id, s.Name })
            .FirstOrDefaultAsync(ct);

        if (student is null)
            return Result<MyAttendanceDto>.Failure("شاگرد یافت نشد یا دسترسی ندارید.");

        var q = _db.Attendances
            .AsNoTracking()
            .Where(a => a.StudentId == request.StudentId);

        if (request.FromDate.HasValue)
            q = q.Where(a => a.Date >= request.FromDate.Value.Date);

        if (request.ToDate.HasValue)
            q = q.Where(a => a.Date <= request.ToDate.Value.Date);

        var rows = await q
            .OrderByDescending(a => a.Date)
            .Select(a => new AttendanceRowDto
            {
                Date = a.Date,
                ClassName = a.Class.Name,
                Status = a.Status,
                Note = a.Note
            })
            .Take(500)
            .ToListAsync(ct);

        return Result<MyAttendanceDto>.Success(new MyAttendanceDto
        {
            StudentId = student.Id,
            StudentName = student.Name,
            Rows = rows
        });
    }
}