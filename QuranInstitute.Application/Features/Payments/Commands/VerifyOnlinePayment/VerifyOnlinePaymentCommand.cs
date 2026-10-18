using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Payments.Commands.VerifyOnlinePayment;

public class VerifyOnlinePaymentCommand : IRequest<Result<Guid>>
{
    public Guid PaymentId { get; set; }
    public VerifyOnlinePaymentCommand(Guid paymentId) => PaymentId = paymentId;
}