using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranInstitute.Application.Features.UserPanel.Queries.GetMyClasses;
using QuranInstitute.Application.Features.Sessions.Queries.GetSessionsByClass;
using QuranInstitute.Web.ViewModels.Sessions;

namespace QuranInstitute.Web.Areas.User.Controllers;

[Area("User")]
[Authorize]
public class SessionsController : Controller
{
    private readonly IMediator _mediator;

    public SessionsController(IMediator mediator) => _mediator = mediator;

    public async Task<IActionResult> Index(Guid classId)
    {
        // ★ بررسی دسترسی: آیا کاربر در این کلاس ثبت‌نام شده؟
        var myClasses = await _mediator.Send(new GetMyClassesQuery());
        if (!myClasses.IsSuccess ||
            !myClasses.Value!.Any(c => c.ClassId == classId))
        {
            TempData["Error"] = "شما در این کلاس ثبت‌نام نکرده‌اید.";
            return RedirectToAction("Index", "Classes");
        }

        var result = await _mediator.Send(new GetSessionsByClassQuery(classId));
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction("Index", "Classes");
        }

        ViewData["Title"] = $"جلسات کلاس {result.Value!.ClassName}";
        return View(new SessionsListViewModel { Data = result.Value! });
    }
}