using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Payments.Queries.GetReceiptData;

public class ReceiptDto
{
    public Guid PaymentId { get; set; }
    public string ReceiptNo { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public string? Note { get; set; }

    public string StudentName { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
    public string ClassName { get; set; } = "—";

    public decimal TotalOwed { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Balance { get; set; }

    public string InstituteName { get; set; } = string.Empty;
}