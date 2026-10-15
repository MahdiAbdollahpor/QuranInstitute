using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuranInstitute.Domain.Entities.Education;

namespace QuranInstitute.Infrastructure.Persistence.Configurations.Education;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> b)
    {
        b.ToTable("Enrollments");
        b.HasKey(x => x.Id);

        b.Property(x => x.Note).HasMaxLength(500);

        b.HasOne(x => x.Student)
         .WithMany(s => s.Enrollments)
         .HasForeignKey(x => x.StudentId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Class)
         .WithMany(c => c.Enrollments)
         .HasForeignKey(x => x.ClassId)
         .OnDelete(DeleteBehavior.Restrict);

        // جلوگیری از ثبت‌نام تکراری در همان کلاس با وضعیت Pending/Approved
        b.HasIndex(x => new { x.StudentId, x.ClassId })
         .HasFilter("[Status] IN (1, 2)")   // Pending=1, Approved=2
         .IsUnique();
    }
}