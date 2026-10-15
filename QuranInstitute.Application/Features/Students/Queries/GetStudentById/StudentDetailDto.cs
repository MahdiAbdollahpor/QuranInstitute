namespace QuranInstitute.Application.Features.Students.Queries.GetStudentById;

public class StudentDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
    public string ParentMobile { get; set; } = string.Empty;
    public Guid? ClassId { get; set; }
    public decimal MonthlyFee { get; set; }
    public DateTime StartDate { get; set; }
    public string? Note { get; set; }
    public bool Active { get; set; }
}