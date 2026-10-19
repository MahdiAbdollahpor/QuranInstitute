using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuranInstitute.Domain.Entities.Communication;

namespace QuranInstitute.Infrastructure.Persistence.Configurations.Communication;

public class SmsLogConfiguration : IEntityTypeConfiguration<SmsLog>
{
    public void Configure(EntityTypeBuilder<SmsLog> b)
    {
        b.ToTable("SmsLogs");
        b.HasKey(x => x.Id);

        b.Property(x => x.Mobile).IsRequired().HasMaxLength(15);
        b.Property(x => x.RecipientName).HasMaxLength(200);
        b.Property(x => x.Message).IsRequired().HasMaxLength(2000);
        b.Property(x => x.ErrorMessage).HasMaxLength(1000);
        b.Property(x => x.ProviderMessageId).HasMaxLength(100);
        b.Property(x => x.Provider).HasMaxLength(50);

        b.HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasIndex(x => x.Mobile);
        b.HasIndex(x => x.Type);
        b.HasIndex(x => x.Status);
        b.HasIndex(x => x.CreatedAt);
    }
}