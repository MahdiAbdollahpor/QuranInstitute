using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AttendanceEntity = QuranInstitute.Domain.Entities.Attendances.Attendance;


namespace QuranInstitute.Infrastructure.Persistence.Configurations.Attendance;

public class AttendanceConfiguration : IEntityTypeConfiguration<AttendanceEntity>
{
    public void Configure(EntityTypeBuilder<AttendanceEntity> b)
    {
        b.ToTable("Attendances");
        b.HasKey(x => x.Id);

        b.Property(x => x.Date).HasColumnType("date");
        b.Property(x => x.Note).HasMaxLength(500);
        b.HasOne(x => x.Class)
         .WithMany(c => c.Attendances)
         .HasForeignKey(x => x.ClassId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Student)
         .WithMany(s => s.Attendances)
         .HasForeignKey(x => x.StudentId)
         .OnDelete(DeleteBehavior.Restrict);

        // جلوگیری از ثبت تکراری برای همان کلاس/شاگرد/روز
        b.HasIndex(x => new { x.ClassId, x.StudentId, x.Date }).IsUnique();

        b.HasIndex(x => new { x.Date, x.Status });
    }
}

