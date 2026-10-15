using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuranInstitute.Domain.Entities.Communication;

namespace QuranInstitute.Infrastructure.Persistence.Configurations.Communication;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> b)
    {
        b.ToTable("Notifications");
        b.HasKey(x => x.Id);

        b.Property(x => x.Title).IsRequired().HasMaxLength(200);
        b.Property(x => x.Body).IsRequired().HasMaxLength(2000);

        b.HasOne(x => x.Student)
         .WithMany(s => s.Notifications)
         .HasForeignKey(x => x.StudentId)
         .OnDelete(DeleteBehavior.SetNull);

        b.HasIndex(x => new { x.UserId, x.IsRead });
        b.HasIndex(x => new { x.StudentId, x.IsRead });
    }
}