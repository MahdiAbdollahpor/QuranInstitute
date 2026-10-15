using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuranInstitute.Domain.Entities.Communication;

namespace QuranInstitute.Infrastructure.Persistence.Configurations.Communication;

public class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
{
    public void Configure(EntityTypeBuilder<Announcement> b)
    {
        b.ToTable("Announcements");
        b.HasKey(x => x.Id);

        b.Property(x => x.Title).IsRequired().HasMaxLength(200);
        b.Property(x => x.Body).IsRequired().HasMaxLength(4000);

        b.HasIndex(x => x.TargetClassId);
        b.HasIndex(x => x.TargetStudentId);
    }
}