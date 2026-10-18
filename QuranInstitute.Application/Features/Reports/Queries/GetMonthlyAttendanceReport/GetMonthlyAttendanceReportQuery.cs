using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Reports.Queries.GetMonthlyAttendanceReport;

public class GetMonthlyAttendanceReportQuery : IRequest<Result<MonthlyAttendanceDto>>
{
    public int JalaliYear { get; set; }
    public int JalaliMonth { get; set; }
    public Guid? ClassId { get; set; }
}