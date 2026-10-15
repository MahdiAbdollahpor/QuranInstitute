using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Students.Queries.GetStudentById;

public class GetStudentByIdQueryHandler
    : IRequestHandler<GetStudentByIdQuery, Result<StudentDetailDto>>
{
    private readonly IApplicationDbContext _db;

    public GetStudentByIdQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<StudentDetailDto>> Handle(
        GetStudentByIdQuery request, CancellationToken ct)
    {
        var s = await _db.Students
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new StudentDetailDto
            {
                Id = x.Id,
                Name = x.Name,
                ParentName = x.ParentName,
                ParentMobile = x.ParentMobile,
                ClassId = x.Enrollments
                    .Where(e => e.Status == Domain.Enums.EnrollmentStatus.Approved)
                    .Select(e => (Guid?)e.ClassId)
                    .FirstOrDefault(),
                MonthlyFee = x.MonthlyFee,
                StartDate = x.StartDate,
                Note = x.Note,
                Active = x.Active
            })
            .FirstOrDefaultAsync(ct);

        if (s is null)
            return Result<StudentDetailDto>.Failure("شاگرد موردنظر یافت نشد.");

        return Result<StudentDetailDto>.Success(s);
    }
}