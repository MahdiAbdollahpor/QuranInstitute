using QuranInstitute.Domain.Enums;

namespace QuranInstitute.Application.Features.Payments.Queries.GetPaymentsList;

public class PaymentListItemDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string ClassName { get; set; } = "—";
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public string ReceiptNo { get; set; } = string.Empty;
    public string? Note { get; set; }
}