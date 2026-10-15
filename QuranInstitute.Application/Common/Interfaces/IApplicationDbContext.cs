using Microsoft.EntityFrameworkCore;
using QuranInstitute.Domain.Entities.Attendances;
using QuranInstitute.Domain.Entities.Communication;
using QuranInstitute.Domain.Entities.Education;
using QuranInstitute.Domain.Entities.Financial;
using QuranInstitute.Domain.Entities.Users;

namespace QuranInstitute.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<AppUser> Users { get; }
    DbSet<AppRole> Roles { get; }

    DbSet<Student> Students { get; }
    DbSet<Class> Classes { get; }
    DbSet<Enrollment> Enrollments { get; }
    DbSet<ClassSession> ClassSessions { get; }
    DbSet<ClassResource> ClassResources { get; }

    DbSet<Attendance> Attendances { get; }
    DbSet<Payment> Payments { get; }

    DbSet<Announcement> Announcements { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<MessageTemplate> MessageTemplates { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}