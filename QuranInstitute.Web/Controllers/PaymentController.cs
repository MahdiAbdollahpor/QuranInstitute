using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranInstitute.Application.Features.Payments.Commands.VerifyOnlinePayment;

namespace QuranInstitute.Web.Controllers;

[Authorize]
public class PaymentController : Controller
{
    private readonly IMediator _mediator;

    public PaymentController(IMediator mediator) => _mediator = mediator;

    // ★★★ هم GET و هم POST ★★★
    [HttpGet]
    [HttpPost]
    public async Task<IActionResult> Callback(Guid paymentId)
    {

        Console.WriteLine($"📥 Callback received: paymentId={paymentId}");
        Console.WriteLine($"📥 QueryString: {Request.QueryString}");
        Console.WriteLine($"📥 Form: {string.Join(",", Request.Form.Select(x => $"{x.Key}={x.Value}"))}");




        if (paymentId == Guid.Empty)
        {
            TempData["Error"] = "شناسه پرداخت نامعتبر است.";
            return RedirectToAction("Index", "Payments", new { area = "User" });
        }

        var result = await _mediator.Send(new VerifyOnlinePaymentCommand(paymentId));

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction("Index", "Payments", new { area = "User" });
        }

        TempData["Success"] = "پرداخت با موفقیت انجام شد.";
        return RedirectToAction("Index", "Payments", new { area = "User" });
    }
}