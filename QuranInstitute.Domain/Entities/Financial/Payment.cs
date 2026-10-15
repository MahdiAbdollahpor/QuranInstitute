using QuranInstitute.Domain.Common;
using QuranInstitute.Domain.Entities.Education;
using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Domain.Entities.Financial;

public class Payment : BaseEntity
{
    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public decimal Amount { get; set; }                        // ریال
    public DateTime Date { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Verified;
    public string? Note { get; set; }

    /// <summary>شماره رسید یکتا — برای چاپ و پیگیری</summary>
    public string ReceiptNo { get; set; } = string.Empty;

    /// <summary>اگر آنلاین باشد، شماره پیگیری درگاه</summary>
    public string? GatewayRef { get; set; }

    public Guid? RecordedBy { get; set; }
}