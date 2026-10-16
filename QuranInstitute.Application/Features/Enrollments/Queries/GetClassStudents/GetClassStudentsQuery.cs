using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Enrollments.Queries.GetClassStudents;

public class GetClassStudentsQuery : IRequest<Result<ClassStudentsResult>>
{
    public Guid ClassId { get; set; }
    public GetClassStudentsQuery(Guid classId) => ClassId = classId;
}

public class ClassStudentsResult
{
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int StudentCount { get; set; }
    public List<ClassStudentDto> Students { get; set; } = new();
}