namespace QuranInstitute.Domain.Enums;

public enum EnrollmentStatus
{
    Pending = 1,   // در انتظار تأیید
    Approved = 2,   // تأیید شده
    Rejected = 3,   // رد شده
    Canceled = 4    // لغو شده توسط کاربر
}