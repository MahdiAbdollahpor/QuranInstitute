using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranInstitute.Application.Features.Payments.Commands.InitOnlinePayment;
using QuranInstitute.Application.Features.UserPanel.Queries.GetMyBalance;
using QuranInstitute.Application.Features.UserPanel.Queries.GetMyProfile;
using QuranInstitute.Web.ViewModels.UserPanel;
using Microsoft.AspNetCore.Http;


namespace QuranInstitute.Web.Areas.User.Controllers;

[Area("User")]
[Authorize]
public class PaymentsController : Controller
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator) => _mediator = mediator;

    public async Task<IActionResult> Index(Guid? studentId = null)
    {
        ViewData["Title"] = "صورتحساب و پرداخت‌ها";

        var profile = await _mediator.Send(new GetMyProfileQuery());
        if (!profile.IsSuccess || !profile.Value!.Students.Any())
        {
            TempData["Error"] = "شاگردی به حساب شما متصل نیست.";
            return RedirectToAction("Index", "Dashboard");
        }

        var sid = studentId ?? profile.Value.Students.First().StudentId;

        var result = await _mediator.Send(new GetMyBalanceQuery(sid));

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction("Index", "Dashboard");
        }

        return View(new MyBalanceViewModel { Data = result.Value! });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> InitOnlinePayment(Guid studentId, decimal amount)
    {
        if (amount <= 0)
        {
            TempData["Error"] = "مبلغ پرداخت نامعتبر است.";
            return RedirectToAction(nameof(Index), new { studentId });
        }

        var callbackBaseUrl = $"{Request.Scheme}://{Request.Host}/Payment/Callback";

        var result = await _mediator.Send(new InitOnlinePaymentCommand(
            studentId, amount, callbackBaseUrl));

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction(nameof(Index), new { studentId });
        }

        // ★★★ Parbad خودش Response را می‌نویسد ★★★
        await result.Value!.GatewayTransporter.TransportAsync();
        // ★★★ بعد از آن، EmptyResult برمی‌گردانیم ★★★
        return new EmptyResult();
    }
}