using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranInstitute.Application.Features.UserPanel.Commands.ChangeMyPassword;
using QuranInstitute.Application.Features.UserPanel.Queries.GetMyProfile;
using QuranInstitute.Web.ViewModels.UserPanel;

namespace QuranInstitute.Web.Areas.User.Controllers;

[Area("User")]
[Authorize]
public class ProfileController : Controller
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator) => _mediator = mediator;

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "پروفایل";

        var result = await _mediator.Send(new GetMyProfileQuery());
        if (!result.IsSuccess)
            return RedirectToAction("Index", "Dashboard");

        ViewBag.Profile = result.Value;
        return View();
    }

    // ─── تغییر رمز ───
    [HttpGet]
    public IActionResult ChangePassword()
    {
        ViewData["Title"] = "تغییر رمز عبور";
        return View(new ChangePasswordViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel vm)
    {
        ViewData["Title"] = "تغییر رمز عبور";

        if (!ModelState.IsValid)
            return View(vm);

        var result = await _mediator.Send(new ChangeMyPasswordCommand
        {
            CurrentPassword = vm.CurrentPassword,
            NewPassword = vm.NewPassword,
            ConfirmPassword = vm.ConfirmPassword
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error!);
            return View(vm);
        }

        TempData["Success"] = "رمز عبور با موفقیت تغییر کرد.";
        return RedirectToAction(nameof(Index));
    }
}