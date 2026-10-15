using QuranInstitute.Domain.Common;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Domain.Entities.Education;

public class Enrollment : BaseEntity
{
    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public Guid ClassId { get; set; }
    public Class Class { get; set; } = null!;

    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Pending;
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? Note { get; set; }
}