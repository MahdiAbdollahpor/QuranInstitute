namespace QuranInstitute.Application.Features.Attendances.Queries.GetAttendanceSummary;

public class AttendanceSummaryDto
{
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Late { get; set; }
    public int Excused { get; set; }
    public int NotRecorded { get; set; }
    public int TotalStudents { get; set; }
    public bool AlreadyRecorded { get; set; }
}