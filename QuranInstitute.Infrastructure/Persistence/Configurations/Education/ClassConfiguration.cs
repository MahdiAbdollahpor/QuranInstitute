using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuranInstitute.Domain.Entities.Education;

namespace QuranInstitute.Infrastructure.Persistence.Configurations.Education;

public class ClassConfiguration : IEntityTypeConfiguration<Class>
{
    public void Configure(EntityTypeBuilder<Class> b)
    {
        b.ToTable("Classes");
        b.HasKey(x => x.Id);

        b.Property(x => x.Name).IsRequired().HasMaxLength(150);
        b.Property(x => x.TeacherName).HasMaxLength(150);
        b.Property(x => x.ClassTime).HasMaxLength(100);
        b.Property(x => x.DaysCsv).HasMaxLength(50);
        b.Property(x => x.DefaultOnlineLink).HasMaxLength(1000);

        // Days به‌عنوان Property محاسباتی نباید map شود
        b.Ignore(x => x.Days);

        b.HasQueryFilter(x => !x.IsDeleted);
    }
}