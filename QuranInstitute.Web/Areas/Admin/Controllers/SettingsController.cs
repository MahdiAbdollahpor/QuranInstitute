using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranInstitute.Application.Features.Settings.Commands.UpdateSettings;
using QuranInstitute.Application.Features.Settings.Queries.GetSettings;
using QuranInstitute.Domain.Constants;
using QuranInstitute.Web.ViewModels.Settings;

namespace QuranInstitute.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
public class SettingsController : MediatorControllerBase
{
    public SettingsController(IMediator mediator) : base(mediator) { }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "تنظیمات موسسه";

        var result = await Mediator.Send(new GetSettingsQuery());
        var s = result.IsSuccess ? result.Value! : new();

        return View(new SettingsViewModel
        {
            InstituteName = s.InstituteName,
            Address = s.Address,
            Phone = s.Phone,
            Email = s.Email,
            LogoUrl = s.LogoUrl,
            AbsentTemplate = s.AbsentTemplate,
            DebtTemplate = s.DebtTemplate,
            GeneralTemplate = s.GeneralTemplate
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(SettingsViewModel vm)
    {
        ViewData["Title"] = "تنظیمات موسسه";

        if (!ModelState.IsValid)
            return View(vm);

        var result = await Mediator.Send(new UpdateSettingsCommand
        {
            InstituteName = vm.InstituteName,
            Address = vm.Address,
            Phone = vm.Phone,
            Email = vm.Email,
            LogoUrl = vm.LogoUrl,
            AbsentTemplate = vm.AbsentTemplate,
            DebtTemplate = vm.DebtTemplate,
            GeneralTemplate = vm.GeneralTemplate
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error!);
            return View(vm);
        }

        TempData["Success"] = "تنظیمات ذخیره شد.";
        return RedirectToAction(nameof(Index));
    }
}