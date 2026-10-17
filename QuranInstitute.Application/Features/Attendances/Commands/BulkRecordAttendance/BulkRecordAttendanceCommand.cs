using MediatR;
using QuranInstitute.Application.Common.Models;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Attendances.Commands.BulkRecordAttendance;

public class BulkRecordAttendanceCommand : IRequest<Result>
{
    public Guid ClassId { get; set; }
    public DateTime Date { get; set; }
    public List<AttendanceInput> Items { get; set; } = new();
}

public class AttendanceInput
{
    public Guid StudentId { get; set; }
    public AttendanceStatus Status { get; set; }
    public string? Note { get; set; }
}