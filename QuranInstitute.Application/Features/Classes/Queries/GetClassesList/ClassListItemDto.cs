using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Classes.Queries.GetClassesList;

public class ClassListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public string? ClassTime { get; set; }
    public int[] Days { get; set; } = Array.Empty<int>();
    public ClassType Type { get; set; }
    public int Capacity { get; set; }
    public int StudentCount { get; set; }
    public bool Active { get; set; }
}