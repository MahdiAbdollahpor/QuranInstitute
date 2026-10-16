using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Enrollments.Queries.GetClassStudents;

public class GetClassStudentsQueryHandler
    : IRequestHandler<GetClassStudentsQuery, Result<ClassStudentsResult>>
{
    private readonly IApplicationDbContext _db;

    public GetClassStudentsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<ClassStudentsResult>> Handle(
        GetClassStudentsQuery request, CancellationToken ct)
    {
        var cls = await _db.Classes
            .AsNoTracking()
            .Where(c => c.Id == request.ClassId)
            .Select(c => new { c.Id, c.Name, c.TeacherName, c.Capacity })
            .FirstOrDefaultAsync(ct);

        if (cls is null)
            return Result<ClassStudentsResult>.Failure("کلاس یافت نشد.");

        var students = await _db.Enrollments
            .AsNoTracking()
            .Where(e => e.ClassId == request.ClassId
                     && e.Status == EnrollmentStatus.Approved)
            .OrderBy(e => e.Student.Name)
            .Select(e => new ClassStudentDto
            {
                EnrollmentId = e.Id,
                StudentId = e.StudentId,
                Name = e.Student.Name,
                ParentName = e.Student.ParentName,
                ParentMobile = e.Student.ParentMobile,
                MonthlyFee = e.Student.MonthlyFee,
                StartDate = e.Student.StartDate,
                ApprovedAt = e.ApprovedAt
            })
            .ToListAsync(ct);

        return Result<ClassStudentsResult>.Success(new ClassStudentsResult
        {
            ClassId = cls.Id,
            ClassName = cls.Name,
            TeacherName = cls.TeacherName,
            Capacity = cls.Capacity,
            StudentCount = students.Count,
            Students = students
        });
    }
}