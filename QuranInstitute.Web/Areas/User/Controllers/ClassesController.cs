using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranInstitute.Application.Features.UserPanel.Commands.RequestEnrollment;
using QuranInstitute.Application.Features.UserPanel.Queries.GetAvailableClasses;
using QuranInstitute.Application.Features.UserPanel.Queries.GetMyClasses;
using QuranInstitute.Application.Features.UserPanel.Queries.GetMyProfile;
using QuranInstitute.Web.ViewModels.UserPanel;

namespace QuranInstitute.Web.Areas.User.Controllers;

[Area("User")]
[Authorize]
public class ClassesController : Controller
{
    private readonly IMediator _mediator;

    public ClassesController(IMediator mediator) => _mediator = mediator;

    // کلاس‌های من
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "کلاس‌های من";

        var result = await _mediator.Send(new GetMyClassesQuery());
        return View(new MyClassesViewModel
        {
            Classes = result.IsSuccess ? result.Value! : new()
        });
    }

    // کلاس‌های موجود برای ثبت‌نام
    public async Task<IActionResult> Available(Guid? studentId = null)
    {
        ViewData["Title"] = "کلاس‌های قابل ثبت‌نام";

        var result = await _mediator.Send(new GetAvailableClassesQuery
        {
            StudentId = studentId
        });

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction(nameof(Index));
        }

        return View(new AvailableClassesViewModel
        {
            Classes = result.Value!,
            SelectedStudentId = studentId
        });
    }

    // ثبت درخواست
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RequestEnrollment(Guid studentId, Guid classId)
    {
        var result = await _mediator.Send(new RequestMyEnrollmentCommand
        {
            StudentId = studentId,
            ClassId = classId
        });

        TempData[result.IsSuccess ? "Success" : "Error"] =
            result.IsSuccess ? "درخواست ثبت‌نام ثبت شد و در انتظار تأیید است."
                             : result.Error;

        return RedirectToAction(nameof(Available));
    }
}