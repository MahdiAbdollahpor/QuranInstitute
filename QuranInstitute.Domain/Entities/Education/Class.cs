using QuranInstitute.Domain.Common;
using QuranInstitute.Domain.Entities.Attendances;
using QuranInstitute.Domain.Enums;


namespace QuranInstitute.Domain.Entities.Education;

public class Class : BaseEntity, ISoftDelete
{
    public string Name { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public Guid? TeacherId { get; set; }                        // لینک اختیاری به AppUser با نقش Teacher
    public string? ClassTime { get; set; }                      // مثل "16:00 تا 17:30"
    public string DaysCsv { get; set; } = string.Empty;         // مثل "6,0,2" → برای EF راحت‌تر
    public ClassType Type { get; set; } = ClassType.InPerson;
    public string? DefaultOnlineLink { get; set; }
    public int Capacity { get; set; } = 30;
    public bool Active { get; set; } = true;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    // ─── Helper برای Days (Computed, Not Mapped) ──────
    public int[] Days
    {
        get => string.IsNullOrWhiteSpace(DaysCsv)
            ? Array.Empty<int>()
            : DaysCsv.Split(',', StringSplitOptions.RemoveEmptyEntries)
                     .Select(int.Parse).ToArray();
        set => DaysCsv = value is null ? string.Empty : string.Join(',', value);
    }

    // Navigation
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<ClassSession> Sessions { get; set; } = new List<ClassSession>();
    public ICollection<ClassResource> Resources { get; set; } = new List<ClassResource>();
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();


}