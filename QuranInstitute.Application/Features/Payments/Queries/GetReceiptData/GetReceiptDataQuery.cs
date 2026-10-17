using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Payments.Queries.GetReceiptData;

public class GetReceiptDataQuery : IRequest<Result<ReceiptDto>>
{
    public Guid PaymentId { get; set; }
    public GetReceiptDataQuery(Guid paymentId) => PaymentId = paymentId;
}