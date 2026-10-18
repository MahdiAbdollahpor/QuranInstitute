using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuranInstitute.Application.Features.Classes.Queries.GetClassById;
using QuranInstitute.Application.Features.Resources.Commands.DeleteResource;
using QuranInstitute.Application.Features.Resources.Commands.UploadResource;
using QuranInstitute.Application.Features.Resources.Queries.GetResourcesByClass;
using QuranInstitute.Application.Features.Sessions.Queries.GetSessionsByClass;
using QuranInstitute.Domain.Constants;
using QuranInstitute.Domain.Enums;
using QuranInstitute.Web.ViewModels.Resources;

namespace QuranInstitute.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]
public class ResourcesController : MediatorControllerBase
{
    public ResourcesController(IMediator mediator) : base(mediator) { }

    // ─── لیست فایل‌های کلاس ───
    [HttpGet]
    public async Task<IActionResult> Index(Guid classId)
    {
        var cls = await Mediator.Send(new GetClassByIdQuery(classId));
        if (!cls.IsSuccess)
        {
            TempData["Error"] = cls.Error;
            return RedirectToAction("Index", "Classes");
        }

        var result = await Mediator.Send(new GetResourcesByClassQuery(classId));
        ViewData["Title"] = $"فایل‌های کلاس {cls.Value!.Name}";

        return View(new ResourcesListViewModel
        {
            ClassId = classId,
            ClassName = cls.Value.Name,
            Resources = result.IsSuccess ? result.Value! : new()
        });
    }

    // ─── آپلود فایل جدید ───
    [HttpGet]
    public async Task<IActionResult> Upload(Guid classId, Guid? sessionId = null)
    {
        var cls = await Mediator.Send(new GetClassByIdQuery(classId));
        if (!cls.IsSuccess)
        {
            TempData["Error"] = cls.Error;
            return RedirectToAction("Index", "Classes");
        }

        ViewData["Title"] = "آپلود فایل جدید";

        var sessions = await Mediator.Send(new GetSessionsByClassQuery(classId));

        return View(new UploadResourceViewModel
        {
            ClassId = classId,
            ClassName = cls.Value!.Name,
            SessionId = sessionId,
            Type = ResourceType.Video,
            Sessions = sessions.IsSuccess
                ? sessions.Value!.Sessions.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"جلسه {s.SessionNumber} - {s.Title}",
                    Selected = sessionId == s.Id
                }).ToList()
                : new(),
            TypeOptions = BuildTypeOptions(ResourceType.Video)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(500 * 1024 * 1024)]
    public async Task<IActionResult> Upload(UploadResourceViewModel vm)
    {
        ViewData["Title"] = "آپلود فایل جدید";

        // ─── اعتبارسنجی دستی بر اساس نوع ───
        if (vm.Type == ResourceType.Link)
        {
            if (string.IsNullOrWhiteSpace(vm.ExternalUrl))
                ModelState.AddModelError(nameof(vm.ExternalUrl), "آدرس لینک الزامی است.");
        }
        else
        {
            if (vm.File is null || vm.File.Length == 0)
                ModelState.AddModelError(nameof(vm.File), "فایل را انتخاب کنید.");
        }

        if (!ModelState.IsValid)
        {
            await RepopulateSelectLists(vm);
            return View(vm);
        }

        using var stream = vm.File?.OpenReadStream();

        var result = await Mediator.Send(new UploadResourceCommand
        {
            ClassId = vm.ClassId,
            SessionId = vm.SessionId,
            Type = vm.Type,
            Title = vm.Title,
            Description = vm.Description,
            EnrolledOnly = vm.EnrolledOnly,
            FileStream = stream,
            FileName = vm.File?.FileName,
            ContentType = vm.File?.ContentType,
            FileSize = vm.File?.Length,
            ExternalUrl = vm.ExternalUrl
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error!);
            await RepopulateSelectLists(vm);
            return View(vm);
        }

        TempData["Success"] = "منبع با موفقیت آپلود شد.";
        return RedirectToAction(nameof(Index), new { classId = vm.ClassId });
    }

    // ─── حذف ───
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, Guid classId)
    {
        var result = await Mediator.Send(new DeleteResourceCommand(id));

        TempData[result.IsSuccess ? "Success" : "Error"] =
            result.IsSuccess ? "منبع حذف شد." : result.Error;

        return RedirectToAction(nameof(Index), new { classId });
    }

    // ─── Helper ───
    private async Task RepopulateSelectLists(UploadResourceViewModel vm)
    {
        var cls = await Mediator.Send(new GetClassByIdQuery(vm.ClassId));
        vm.ClassName = cls.IsSuccess ? cls.Value!.Name : "";

        var sessions = await Mediator.Send(new GetSessionsByClassQuery(vm.ClassId));
        vm.Sessions = sessions.IsSuccess
            ? sessions.Value!.Sessions.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = $"جلسه {s.SessionNumber} - {s.Title}",
                Selected = vm.SessionId == s.Id
            }).ToList()
            : new();

        vm.TypeOptions = BuildTypeOptions(vm.Type);
    }

    private static List<SelectListItem> BuildTypeOptions(ResourceType selected)
    {
        return new List<SelectListItem>
        {
            new() { Value = ((int)ResourceType.Video).ToString(),
                    Text = "فیلم", Selected = selected == ResourceType.Video },
            new() { Value = ((int)ResourceType.Audio).ToString(),
                    Text = "صوت", Selected = selected == ResourceType.Audio },
            new() { Value = ((int)ResourceType.Pdf).ToString(),
                    Text = "جزوه PDF", Selected = selected == ResourceType.Pdf },
            new() { Value = ((int)ResourceType.Image).ToString(),
                    Text = "تصویر", Selected = selected == ResourceType.Image },
            new() { Value = ((int)ResourceType.Link).ToString(),
                    Text = "لینک خارجی", Selected = selected == ResourceType.Link },
            new() { Value = ((int)ResourceType.Other).ToString(),
                    Text = "سایر", Selected = selected == ResourceType.Other }
        };
    }
}