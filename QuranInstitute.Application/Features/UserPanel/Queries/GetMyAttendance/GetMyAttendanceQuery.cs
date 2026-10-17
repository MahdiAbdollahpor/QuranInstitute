using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.UserPanel.Queries.GetMyAttendance;

public class GetMyAttendanceQuery : IRequest<Result<MyAttendanceDto>>
{
    public Guid StudentId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public GetMyAttendanceQuery(Guid studentId)
    {
        StudentId = studentId;
    }
}