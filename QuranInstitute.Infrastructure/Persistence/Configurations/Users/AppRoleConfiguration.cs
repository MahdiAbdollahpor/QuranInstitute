using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuranInstitute.Domain.Entities.Users;

namespace QuranInstitute.Infrastructure.Persistence.Configurations.Users;

public class AppRoleConfiguration : IEntityTypeConfiguration<AppRole>
{
    public void Configure(EntityTypeBuilder<AppRole> b)
    {
        b.Property(x => x.Description).HasMaxLength(300);
    }
}