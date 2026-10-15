using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuranInstitute.Domain.Entities.Education;

namespace QuranInstitute.Infrastructure.Persistence.Configurations.Education;

public class ClassResourceConfiguration : IEntityTypeConfiguration<ClassResource>
{
    public void Configure(EntityTypeBuilder<ClassResource> b)
    {
        b.ToTable("ClassResources");
        b.HasKey(x => x.Id);

        b.Property(x => x.Title).IsRequired().HasMaxLength(200);
        b.Property(x => x.Description).HasMaxLength(2000);
        b.Property(x => x.FileUrl).IsRequired().HasMaxLength(1000);
        b.Property(x => x.MimeType).HasMaxLength(100);

        b.HasOne(x => x.Class)
         .WithMany(c => c.Resources)
         .HasForeignKey(x => x.ClassId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Session)
 .WithMany(s => s.Resources)
 .HasForeignKey(x => x.SessionId)
 .OnDelete(DeleteBehavior.NoAction);

        b.HasIndex(x => x.ClassId);
        b.HasIndex(x => x.Type);
    }
}