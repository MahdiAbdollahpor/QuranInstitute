using QuranInstitute.Domain.Common;

namespace QuranInstitute.Domain.Entities.Education;

public class ClassSession : BaseEntity
{
    public Guid ClassId { get; set; }
    public Class Class { get; set; } = null!;

    public int SessionNumber { get; set; }
    public DateTime Date { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsOnline { get; set; }
    public string? OnlineLink { get; set; }
    public string? RecordingUrl { get; set; }

    // Navigation
    public ICollection<ClassResource> Resources { get; set; } = new List<ClassResource>();
}