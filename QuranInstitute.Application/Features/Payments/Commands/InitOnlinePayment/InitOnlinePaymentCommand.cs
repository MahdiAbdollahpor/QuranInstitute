using MediatR;
using Parbad;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Payments.Commands.InitOnlinePayment;

public class InitOnlinePaymentCommand : IRequest<Result<IPaymentRequestResult>>
{
    public Guid StudentId { get; set; }
    public decimal Amount { get; set; }
    public string CallbackBaseUrl { get; set; } = string.Empty;

    public InitOnlinePaymentCommand(Guid studentId, decimal amount, string callbackBaseUrl)
    {
        StudentId = studentId;
        Amount = amount;
        CallbackBaseUrl = callbackBaseUrl;
    }
}