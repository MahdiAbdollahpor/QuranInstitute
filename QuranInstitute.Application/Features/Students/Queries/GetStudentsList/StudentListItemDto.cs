namespace QuranInstitute.Application.Features.Students.Queries.GetStudentsList;

public class StudentListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
    public string ParentMobile { get; set; } = string.Empty;
    public Guid? ClassId { get; set; }
    public string ClassName { get; set; } = "—";
    public decimal MonthlyFee { get; set; }
    public DateTime StartDate { get; set; }
    public bool Active { get; set; }
}