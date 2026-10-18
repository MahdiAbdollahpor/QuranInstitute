using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranInstitute.Application.Features.Classes.Queries.GetClassById;
using QuranInstitute.Application.Features.Sessions.Commands.CreateSession;
using QuranInstitute.Application.Features.Sessions.Commands.DeleteSession;
using QuranInstitute.Application.Features.Sessions.Commands.UpdateSession;
using QuranInstitute.Application.Features.Sessions.Queries.GetSessionById;
using QuranInstitute.Application.Features.Sessions.Queries.GetSessionsByClass;
using QuranInstitute.Domain.Constants;
using QuranInstitute.Web.ViewModels.Sessions;

namespace QuranInstitute.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]
public class SessionsController : MediatorControllerBase
{
    public SessionsController(IMediator mediator) : base(mediator) { }

    // ─── لیست جلسات یک کلاس ───
    [HttpGet]
    public async Task<IActionResult> Index(Guid classId)
    {
        var result = await Mediator.Send(new GetSessionsByClassQuery(classId));
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction("Index", "Classes");
        }

        ViewData["Title"] = $"جلسات کلاس {result.Value!.ClassName}";
        return View(new SessionsListViewModel { Data = result.Value! });
    }

    // ─── ساخت جلسه ───
    [HttpGet]
    public async Task<IActionResult> Create(Guid classId)
    {
        var cls = await Mediator.Send(new GetClassByIdQuery(classId));
        if (!cls.IsSuccess)
        {
            TempData["Error"] = cls.Error;
            return RedirectToAction("Index", "Classes");
        }

        // شماره جلسه بعدی
        var sessions = await Mediator.Send(new GetSessionsByClassQuery(classId));
        var nextNumber = sessions.IsSuccess && sessions.Value!.Sessions.Any()
            ? sessions.Value.Sessions.Max(s => s.SessionNumber) + 1
            : 1;

        ViewData["Title"] = "تعریف جلسه جدید";

        return View(new CreateSessionViewModel
        {
            ClassId = classId,
            ClassName = cls.Value!.Name,
            SessionNumber = nextNumber
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSessionViewModel vm)
    {
        ViewData["Title"] = "تعریف جلسه جدید";

        if (!ModelState.IsValid)
            return View(vm);

        var result = await Mediator.Send(new CreateSessionCommand
        {
            ClassId = vm.ClassId,
            SessionNumber = vm.SessionNumber,
            Date = vm.Date,
            Title = vm.Title,
            Description = vm.Description,
            IsOnline = vm.IsOnline,
            OnlineLink = vm.OnlineLink,
            RecordingUrl = vm.RecordingUrl
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error!);
            return View(vm);
        }

        TempData["Success"] = "جلسه با موفقیت ثبت شد.";
        return RedirectToAction(nameof(Index), new { classId = vm.ClassId });
    }

    // ─── ویرایش ───
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var result = await Mediator.Send(new GetSessionByIdQuery(id));
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction("Index", "Classes");
        }

        var s = result.Value!;
        ViewData["Title"] = "ویرایش جلسه";

        return View(new EditSessionViewModel
        {
            Id = s.Id,
            ClassId = s.ClassId,
            ClassName = s.ClassName,
            SessionNumber = s.SessionNumber,
            Date = s.Date,
            Title = s.Title,
            Description = s.Description,
            IsOnline = s.IsOnline,
            OnlineLink = s.OnlineLink,
            RecordingUrl = s.RecordingUrl
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditSessionViewModel vm)
    {
        ViewData["Title"] = "ویرایش جلسه";

        if (!ModelState.IsValid)
            return View(vm);

        var result = await Mediator.Send(new UpdateSessionCommand
        {
            Id = vm.Id,
            SessionNumber = vm.SessionNumber,
            Date = vm.Date,
            Title = vm.Title,
            Description = vm.Description,
            IsOnline = vm.IsOnline,
            OnlineLink = vm.OnlineLink,
            RecordingUrl = vm.RecordingUrl
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error!);
            return View(vm);
        }

        TempData["Success"] = "تغییرات ذخیره شد.";
        return RedirectToAction(nameof(Index), new { classId = vm.ClassId });
    }

    // ─── حذف ───
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, Guid classId)
    {
        var result = await Mediator.Send(new DeleteSessionCommand(id));

        TempData[result.IsSuccess ? "Success" : "Error"] =
            result.IsSuccess ? "جلسه حذف شد." : result.Error;

        return RedirectToAction(nameof(Index), new { classId });
    }
}