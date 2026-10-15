using QuranInstitute.Domain.Common;
using QuranInstitute.Domain.Entities.Communication;
using QuranInstitute.Domain.Entities.Financial;
using QuranInstitute.Domain.Entities.Users;
using QuranInstitute.Domain.ValueObjects;
using QuranInstitute.Domain.Entities.Attendances;

namespace QuranInstitute.Domain.Entities.Education;

public class Student : BaseEntity, ISoftDelete
{
    public string Name { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
    public string ParentMobile { get; set; } = string.Empty;   // ValueObject در Config map می‌شود
    public decimal MonthlyFee { get; set; }                    // ریال
    public DateTime StartDate { get; set; }                    // تاریخ شروع محاسبه شهریه
    public string? Note { get; set; }
    public bool Active { get; set; } = true;

    // لینک اختیاری به کاربر (اگر شاگرد خودش لاگین کند)
    public Guid? UserId { get; set; }
    public AppUser? User { get; set; }

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigation
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}