using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuranInstitute.Domain.Entities.Education;

namespace QuranInstitute.Infrastructure.Persistence.Configurations.Education;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> b)
    {
        b.ToTable("Students");
        b.HasKey(x => x.Id);

        b.Property(x => x.Name).IsRequired().HasMaxLength(150);
        b.Property(x => x.ParentName).IsRequired().HasMaxLength(150);
        b.Property(x => x.ParentMobile).IsRequired().HasMaxLength(15);
        b.Property(x => x.MonthlyFee).HasColumnType("decimal(18,0)");
        b.Property(x => x.Note).HasMaxLength(1000);

        b.HasIndex(x => x.ParentMobile);
        b.HasIndex(x => x.Active);

        // Query Filter: حذف‌شده‌ها به‌صورت پیش‌فرض نمایش داده نشوند
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}