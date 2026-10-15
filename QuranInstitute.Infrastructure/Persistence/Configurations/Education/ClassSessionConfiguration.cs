using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuranInstitute.Domain.Entities.Education;

namespace QuranInstitute.Infrastructure.Persistence.Configurations.Education;

public class ClassSessionConfiguration : IEntityTypeConfiguration<ClassSession>
{
    public void Configure(EntityTypeBuilder<ClassSession> b)
    {
        b.ToTable("ClassSessions");
        b.HasKey(x => x.Id);

        b.Property(x => x.Title).IsRequired().HasMaxLength(200);
        b.Property(x => x.Description).HasMaxLength(2000);
        b.Property(x => x.OnlineLink).HasMaxLength(1000);
        b.Property(x => x.RecordingUrl).HasMaxLength(1000);

        b.HasOne(x => x.Class)
         .WithMany(c => c.Sessions)
         .HasForeignKey(x => x.ClassId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => new { x.ClassId, x.SessionNumber }).IsUnique();
    }
}