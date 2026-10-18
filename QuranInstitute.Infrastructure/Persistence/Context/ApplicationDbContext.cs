using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Domain.Common;
using QuranInstitute.Domain.Entities.Attendances;
using QuranInstitute.Domain.Entities.Communication;
using QuranInstitute.Domain.Entities.Education;
using QuranInstitute.Domain.Entities.Financial;
using QuranInstitute.Domain.Entities.System;
using QuranInstitute.Domain.Entities.Users;
using System.Reflection;

namespace QuranInstitute.Infrastructure.Persistence.Context;

public class ApplicationDbContext
    : IdentityDbContext<AppUser, AppRole, Guid>, IApplicationDbContext
{
    private readonly ICurrentUserService? _currentUser;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService? currentUser = null)
        : base(options)
    {
        _currentUser = currentUser;
    }

    // ─── DbSets ─────────────────────────────────────────
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<ClassSession> ClassSessions => Set<ClassSession>();
    public DbSet<ClassResource> ClassResources => Set<ClassResource>();

    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<AppSettings> AppSettings => Set<AppSettings>();

    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<MessageTemplate> MessageTemplates => Set<MessageTemplate>();

    // ─── Configurations ────────────────────────────────
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // همه Configuration ها را از همین Assembly اسکن کن
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // نام‌گذاری جداول Identity
        builder.Entity<AppUser>().ToTable("Users");
        builder.Entity<AppRole>().ToTable("Roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
    }

    // ─── Auto Audit ────────────────────────────────────
    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        ApplyAuditInfo();
        return base.SaveChangesAsync(ct);
    }

    public override int SaveChanges()
    {
        ApplyAuditInfo();
        return base.SaveChanges();
    }

    private void ApplyAuditInfo()
    {
        var now = DateTime.UtcNow;
        var userId = _currentUser?.UserId;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = userId;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = userId;
                    break;
            }
        }
    }
}