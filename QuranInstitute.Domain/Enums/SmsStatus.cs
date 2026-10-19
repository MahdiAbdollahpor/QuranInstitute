namespace QuranInstitute.Domain.Enums;

public enum SmsStatus
{
    Pending = 1,    // در انتظار
    Sent = 2,       // ارسال موفق
    Failed = 3,     // ارسال ناموفق
    Delivered = 4   // تحویل داده شد (از API جداگانه)
}