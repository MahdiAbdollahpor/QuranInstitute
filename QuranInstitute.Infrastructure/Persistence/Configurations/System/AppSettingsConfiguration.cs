using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuranInstitute.Domain.Entities.System;

namespace QuranInstitute.Infrastructure.Persistence.Configurations.System;

public class AppSettingsConfiguration : IEntityTypeConfiguration<AppSettings>
{
    public void Configure(EntityTypeBuilder<AppSettings> b)
    {
        b.ToTable("AppSettings");
        b.HasKey(x => x.Id);

        b.Property(x => x.InstituteName).IsRequired().HasMaxLength(200);
        b.Property(x => x.Address).HasMaxLength(500);
        b.Property(x => x.Phone).HasMaxLength(20);
        b.Property(x => x.Email).HasMaxLength(150);
        b.Property(x => x.LogoUrl).HasMaxLength(500);
        b.Property(x => x.AbsentTemplate).HasMaxLength(2000);
        b.Property(x => x.DebtTemplate).HasMaxLength(2000);
        b.Property(x => x.GeneralTemplate).HasMaxLength(2000);
    }
}