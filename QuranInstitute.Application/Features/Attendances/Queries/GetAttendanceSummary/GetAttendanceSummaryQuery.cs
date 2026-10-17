using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Attendances.Queries.GetAttendanceSummary;

public class GetAttendanceSummaryQuery : IRequest<Result<AttendanceSummaryDto>>
{
    public Guid ClassId { get; set; }
    public DateTime Date { get; set; }

    public GetAttendanceSummaryQuery(Guid classId, DateTime date)
    {
        ClassId = classId;
        Date = date;
    }
}