using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.UserPanel.Queries.GetAvailableClasses;

public class AvailableClassDto
{
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public string? ClassTime { get; set; }
    public int[] Days { get; set; } = Array.Empty<int>();
    public ClassType Type { get; set; }
    public int Capacity { get; set; }
    public int StudentCount { get; set; }
    public bool IsFull => Capacity > 0 && StudentCount >= Capacity;
    public bool IsEnrolled { get; set; }
}