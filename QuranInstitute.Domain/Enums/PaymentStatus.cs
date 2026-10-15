namespace QuranInstitute.Domain.Enums;

public enum PaymentStatus
{
    Pending = 1,   // در انتظار (پرداخت آنلاین)
    Verified = 2,   // تأیید شده
    Rejected = 3    // رد شده
}