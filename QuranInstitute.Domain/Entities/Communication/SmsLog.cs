using QuranInstitute.Domain.Common;
using QuranInstitute.Domain.Entities.Education;
using QuranInstitute.Domain.Enums;
using System.Runtime.InteropServices.JavaScript;

namespace QuranInstitute.Domain.Entities.Communication;

/// <summary>
/// تاریخچه پیامک‌های ارسال‌شده — هر رکورد یک پیامک به یک شماره
/// </summary>
public class SmsLog : BaseEntity
{
    /// <summary>شاگرد مرتبط (اگر پیامک به والدین شاگرد باشد)</summary>
    public Guid? StudentId { get; set; }
    public Student? Student { get; set; }

    /// <summary>شماره موبایل گیرنده (نرمال‌شده)</summary>
    public string Mobile { get; set; } = string.Empty;

    /// <summary>نام گیرنده (برای خوانایی در تاریخچه)</summary>
    public string? RecipientName { get; set; }

    /// <summary>متن نهایی پیامک (بعد از جایگزینی متغیرها)</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>نوع پیامک — برای فیلتر و آمار</summary>
    public SmsType Type { get; set; }

    /// <summary>وضعیت ارسال</summary>
    public SmsStatus Status { get; set; } = SmsStatus.Pending;

    /// <summary>پیام خطا (اگر ناموفق)</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>شناسه پیام در پنل کاوه‌نگار (اگر موفق)</summary>
    public string? ProviderMessageId { get; set; }

    /// <summary>نام درگاه (Null, Kavenegar, ...)</summary>
    public string Provider { get; set; } = "Null";

    /// <summary>تاریخ ارسال موفق</summary>
    public DateTime? SentAt { get; set; }

    /// <summary>چه کسی ارسال کرد</summary>
    public Guid? SentByUserId { get; set; }
}