using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Payments.Commands.DeletePayment;

public class DeletePaymentCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    public DeletePaymentCommand(Guid id) => Id = id;
}