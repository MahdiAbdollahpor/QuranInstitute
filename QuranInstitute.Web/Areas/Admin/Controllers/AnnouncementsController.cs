using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuranInstitute.Application.Features.Classes.Queries.GetClassesList;
using QuranInstitute.Application.Features.Notifications.Commands.CreateAnnouncement;
using QuranInstitute.Application.Features.Notifications.Commands.DeleteAnnouncement;
using QuranInstitute.Application.Features.Notifications.Queries.GetAnnouncementById;
using QuranInstitute.Application.Features.Notifications.Queries.GetAnnouncementsList;
using QuranInstitute.Application.Features.Students.Queries.GetStudentsList;
using QuranInstitute.Domain.Constants;
using QuranInstitute.Web.ViewModels.Announcements;

namespace QuranInstitute.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]
public class AnnouncementsController : MediatorControllerBase
{
    public AnnouncementsController(IMediator mediator) : base(mediator) { }

    // ─── لیست اطلاعیه‌ها ───
    [HttpGet]
    public async Task<IActionResult> Index(string? search = null, int page = 1)
    {
        ViewData["Title"] = "اطلاعیه‌ها";

        var result = await Mediator.Send(new GetAnnouncementsListQuery
        {
            Search = search,
            Page = page,
            PageSize = 20
        });

        return View(new AnnouncementListViewModel
        {
            Announcements = result,
            Search = search
        });
    }

    // ─── ساخت اطلاعیه ───
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "اطلاعیه جدید";

        return View(await BuildCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateAnnouncementViewModel vm)
    {
        ViewData["Title"] = "اطلاعیه جدید";

        if (!ModelState.IsValid)
            return View(await BuildCreateViewModel(vm));

        var result = await Mediator.Send(new CreateAnnouncementCommand
        {
            Title = vm.Title,
            Body = vm.Body,
            TargetClassId = vm.TargetClassId,
            TargetStudentId = vm.TargetStudentId,
            SendSms = vm.SendSms,
            SendWhatsApp = vm.SendWhatsApp,
            SendInApp = vm.SendInApp
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error!);
            return View(await BuildCreateViewModel(vm));
        }

        TempData["Success"] = "اطلاعیه با موفقیت ارسال شد.";
        return RedirectToAction(nameof(Details), new { id = result.Value });
    }

    // ─── جزئیات ───
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var result = await Mediator.Send(new GetAnnouncementByIdQuery(id));
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction(nameof(Index));
        }

        ViewData["Title"] = "جزئیات اطلاعیه";
        return View(result.Value!);
    }

    // ─── حذف ───
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await Mediator.Send(new DeleteAnnouncementCommand(id));

        TempData[result.IsSuccess ? "Success" : "Error"] =
            result.IsSuccess ? "اطلاعیه حذف شد." : result.Error;

        return RedirectToAction(nameof(Index));
    }

    // ─── Helper ───
    private async Task<CreateAnnouncementViewModel> BuildCreateViewModel(
        CreateAnnouncementViewModel? existing = null)
    {
        var classes = await Mediator.Send(new GetClassesListQuery
        {
            ActiveOnly = true,
            PageSize = 500
        });

        var students = await Mediator.Send(new GetStudentsListQuery
        {
            ActiveOnly = true,
            PageSize = 500
        });

        var vm = existing ?? new CreateAnnouncementViewModel();

        vm.Classes = classes.Items.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name,
            Selected = vm.TargetClassId == c.Id
        }).ToList();

        vm.Students = students.Items.Select(s => new SelectListItem
        {
            Value = s.Id.ToString(),
            Text = s.Name,
            Selected = vm.TargetStudentId == s.Id
        }).ToList();

        return vm;
    }
}