using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Attendances.Queries.GetAttendanceByClassDate;

public class GetAttendanceByClassDateQuery : IRequest<Result<AttendanceFormResult>>
{
    public Guid ClassId { get; set; }
    public DateTime Date { get; set; }

    public GetAttendanceByClassDateQuery(Guid classId, DateTime date)
    {
        ClassId = classId;
        Date = date;
    }
}

public class AttendanceFormResult
{
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public int[] Days { get; set; } = Array.Empty<int>();
    public DateTime Date { get; set; }
    public bool IsClassDay { get; set; }
    public bool AlreadyRecorded { get; set; }
    public List<AttendanceEntryDto> Entries { get; set; } = new();
}