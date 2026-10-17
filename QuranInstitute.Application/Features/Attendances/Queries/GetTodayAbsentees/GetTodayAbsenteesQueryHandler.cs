using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Attendances.Queries.GetTodayAbsentees;

public class GetTodayAbsenteesQueryHandler
    : IRequestHandler<GetTodayAbsenteesQuery, List<TodaysAbsenteeDto>>
{
    private readonly IApplicationDbContext _db;

    public GetTodayAbsenteesQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<List<TodaysAbsenteeDto>> Handle(
        GetTodayAbsenteesQuery request, CancellationToken ct)
    {
        var date = (request.Date ?? DateTime.Today).Date;

        return await _db.Attendances
            .AsNoTracking()
            .Where(a => a.Date == date && a.Status == AttendanceStatus.Absent)
            .OrderBy(a => a.Class.Name)
            .ThenBy(a => a.Student.Name)
            .Select(a => new TodaysAbsenteeDto
            {
                StudentId = a.StudentId,
                StudentName = a.Student.Name,
                ParentName = a.Student.ParentName,
                ParentMobile = a.Student.ParentMobile,
                ClassId = a.ClassId,
                ClassName = a.Class.Name
            })
            .ToListAsync(ct);
    }
}