using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranInstitute.Application.Features.UserPanel.Commands.MarkNotificationRead;
using QuranInstitute.Application.Features.UserPanel.Queries.GetMyNotifications;
using QuranInstitute.Web.ViewModels.UserPanel;

namespace QuranInstitute.Web.Areas.User.Controllers;

[Area("User")]
[Authorize]
public class NotificationsController : Controller
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator) => _mediator = mediator;

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "اطلاعیه‌ها";

        var result = await _mediator.Send(new GetMyNotificationsQuery());

        return View(new MyNotificationsViewModel
        {
            Notifications = result.IsSuccess ? result.Value! : new()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkRead(Guid id)
    {
        await _mediator.Send(new MarkNotificationReadCommand(id));
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAllRead()
    {
        await _mediator.Send(new MarkAllNotificationsReadCommand());
        return RedirectToAction(nameof(Index));
    }
}