namespace QuranInstitute.Application.Features.Enrollments.Queries.GetClassStudents;

public class ClassStudentDto
{
    public Guid EnrollmentId { get; set; }
    public Guid StudentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
    public string ParentMobile { get; set; } = string.Empty;
    public decimal MonthlyFee { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? ApprovedAt { get; set; }
}