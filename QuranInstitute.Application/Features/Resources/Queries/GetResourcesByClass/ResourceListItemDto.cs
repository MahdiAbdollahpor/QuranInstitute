using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Resources.Queries.GetResourcesByClass;

public class ResourceListItemDto
{
    public Guid Id { get; set; }
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public Guid? SessionId { get; set; }
    public int? SessionNumber { get; set; }
    public ResourceType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string PublicUrl { get; set; } = string.Empty;
    public long? FileSize { get; set; }
    public string? MimeType { get; set; }
    public bool EnrolledOnly { get; set; }
    public DateTime CreatedAt { get; set; }
}