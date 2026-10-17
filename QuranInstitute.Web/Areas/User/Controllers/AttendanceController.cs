using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranInstitute.Application.Features.UserPanel.Queries.GetMyAttendance;
using QuranInstitute.Application.Features.UserPanel.Queries.GetMyProfile;
using QuranInstitute.Web.ViewModels.UserPanel;

namespace QuranInstitute.Web.Areas.User.Controllers;

[Area("User")]
[Authorize]
public class AttendanceController : Controller
{
    private readonly IMediator _mediator;

    public AttendanceController(IMediator mediator) => _mediator = mediator;

    public async Task<IActionResult> Index(Guid? studentId = null)
    {
        ViewData["Title"] = "حضور و غیاب";

        var profile = await _mediator.Send(new GetMyProfileQuery());
        if (!profile.IsSuccess || !profile.Value!.Students.Any())
        {
            TempData["Error"] = "شاگردی به حساب شما متصل نیست.";
            return RedirectToAction("Index", "Dashboard");
        }

        var sid = studentId ?? profile.Value.Students.First().StudentId;

        var result = await _mediator.Send(new GetMyAttendanceQuery(sid));

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction("Index", "Dashboard");
        }

        return View(new MyAttendanceViewModel
        {
            Data = result.Value!,
            StudentId = sid
        });
    }
}