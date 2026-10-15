using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuranInstitute.Domain.Entities.Users;

namespace QuranInstitute.Infrastructure.Persistence.Configurations.Users;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> b)
    {
        b.Property(x => x.FullName).IsRequired().HasMaxLength(150);
        b.Property(x => x.NationalCode).HasMaxLength(10);
        b.Property(x => x.AvatarUrl).HasMaxLength(500);

        b.HasIndex(x => x.NationalCode).IsUnique().HasFilter("[NationalCode] IS NOT NULL");

        b.HasMany(x => x.Students)
         .WithOne(s => s.User)
         .HasForeignKey(s => s.UserId)
         .OnDelete(DeleteBehavior.SetNull);
    }
}