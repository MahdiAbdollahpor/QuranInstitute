using QuranInstitute.Domain.Common;

namespace QuranInstitute.Domain.Entities.Communication;

/// <summary>
/// قالب پیام‌ها — به‌جای ذخیره در Settings، جدول جدا داریم.
/// </summary>
public class MessageTemplate : BaseEntity
{
    /// <summary>مثل: absent, debt, general</summary>
    public string Key { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}