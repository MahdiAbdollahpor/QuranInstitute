using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Payments.Commands.DeletePayment;

public class DeletePaymentCommandHandler
    : IRequestHandler<DeletePaymentCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public DeletePaymentCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(DeletePaymentCommand request, CancellationToken ct)
    {
        var payment = await _db.Payments
            .FirstOrDefaultAsync(p => p.Id == request.Id, ct);

        if (payment is null)
            return Result.Failure("پرداخت یافت نشد.");

        _db.Payments.Remove(payment);
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}