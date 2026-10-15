namespace QuranInstitute.Domain.Common;

/// <summary>
/// موجودیتی که برای Audit نیاز به CreatedBy/UpdatedBy دارد.
/// از BaseEntity ارث می‌برد و فقط برای معناداری اضافه شده.
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
}