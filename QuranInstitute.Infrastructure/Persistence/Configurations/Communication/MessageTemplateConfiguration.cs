using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuranInstitute.Domain.Entities.Communication;

namespace QuranInstitute.Infrastructure.Persistence.Configurations.Communication;

public class MessageTemplateConfiguration : IEntityTypeConfiguration<MessageTemplate>
{
    public void Configure(EntityTypeBuilder<MessageTemplate> b)
    {
        b.ToTable("MessageTemplates");
        b.HasKey(x => x.Id);

        b.Property(x => x.Key).IsRequired().HasMaxLength(50);
        b.Property(x => x.Title).IsRequired().HasMaxLength(150);
        b.Property(x => x.Body).IsRequired().HasMaxLength(2000);

        b.HasIndex(x => x.Key).IsUnique();
    }
}