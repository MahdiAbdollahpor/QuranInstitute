using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Enrollments.Queries.GetEnrollmentsList;

public class EnrollmentListItemDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string ParentMobile { get; set; } = string.Empty;
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public EnrollmentStatus Status { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? Note { get; set; }
}