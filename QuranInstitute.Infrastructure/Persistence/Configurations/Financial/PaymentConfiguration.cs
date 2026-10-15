using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuranInstitute.Domain.Entities.Financial;

namespace QuranInstitute.Infrastructure.Persistence.Configurations.Financial;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> b)
    {
        b.ToTable("Payments");
        b.HasKey(x => x.Id);

        b.Property(x => x.Amount).HasColumnType("decimal(18,0)");
        b.Property(x => x.Date).HasColumnType("date");
        b.Property(x => x.ReceiptNo).IsRequired().HasMaxLength(30);
        b.Property(x => x.GatewayRef).HasMaxLength(100);
        b.Property(x => x.Note).HasMaxLength(1000);

        b.HasOne(x => x.Student)
         .WithMany(s => s.Payments)
         .HasForeignKey(x => x.StudentId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.ReceiptNo).IsUnique();
        b.HasIndex(x => new { x.StudentId, x.Date });
        b.HasIndex(x => x.Status);
    }
}