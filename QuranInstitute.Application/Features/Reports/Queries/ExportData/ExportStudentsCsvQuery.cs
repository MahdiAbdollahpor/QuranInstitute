using MediatR;

namespace QuranInstitute.Application.Features.Reports.Queries.ExportData;

public class ExportStudentsCsvQuery : IRequest<string>
{
    public bool ActiveOnly { get; set; } = true;
}

public class ExportPaymentsCsvQuery : IRequest<string>
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class ExportAttendanceCsvQuery : IRequest<string>
{
    public int JalaliYear { get; set; }
    public int JalaliMonth { get; set; }
}