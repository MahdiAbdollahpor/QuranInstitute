using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuranInstitute.Application.Features.Admin.Users.Commands.ActivateUser;
using QuranInstitute.Application.Features.Admin.Users.Commands.ChangeUserRole;
using QuranInstitute.Application.Features.Admin.Users.Commands.DeactivateUser;
using QuranInstitute.Application.Features.Admin.Users.Commands.DeleteUser;
using QuranInstitute.Application.Features.Admin.Users.Commands.ResetUserPassword;
using QuranInstitute.Application.Features.Admin.Users.Queries.GetUserDetails;
using QuranInstitute.Application.Features.Admin.Users.Queries.GetUsersList;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Domain.Constants;
using QuranInstitute.Web.ViewModels.Admin;

namespace QuranInstitute.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
public class UsersController : MediatorControllerBase
{
    private readonly ICurrentUserService _current;

    public UsersController(IMediator mediator, ICurrentUserService current)
        : base(mediator)
    {
        _current = current;
    }

    // ─── لیست کاربران ───
    [HttpGet]
    public async Task<IActionResult> Index(
        string? search = null,
        string? role = null,
        bool? activeOnly = null,
        int page = 1)
    {
        ViewData["Title"] = "کاربران";

        var result = await Mediator.Send(new GetUsersListQuery
        {
            Search = search,
            RoleFilter = role,
            ActiveOnly = activeOnly,
            Page = page,
            PageSize = 30
        });

        return View(new UserListViewModel
        {
            Users = result.IsSuccess ? result.Value! : new(),
            Search = search,
            RoleFilter = role,
            ActiveOnly = activeOnly,
            RoleOptions = BuildRoleOptions(role)
        });
    }

    // ─── جزئیات ───
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var result = await Mediator.Send(new GetUserDetailsQuery(id));
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction(nameof(Index));
        }

        ViewData["Title"] = $"کاربر: {result.Value!.FullName}";

        return View(new UserDetailsViewModel
        {
            User = result.Value!,
            IsCurrentUser = _current.UserId == id
        });
    }

    // ─── غیرفعال‌سازی ───
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var result = await Mediator.Send(new DeactivateUserCommand(id));

        TempData[result.IsSuccess ? "Success" : "Error"] =
            result.IsSuccess ? "کاربر غیرفعال شد." : result.Error;

        return RedirectToAction(nameof(Details), new { id });
    }

    // ─── فعال‌سازی ───
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(Guid id)
    {
        var result = await Mediator.Send(new ActivateUserCommand(id));

        TempData[result.IsSuccess ? "Success" : "Error"] =
            result.IsSuccess ? "کاربر فعال شد." : result.Error;

        return RedirectToAction(nameof(Details), new { id });
    }

    // ─── حذف ───
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await Mediator.Send(new DeleteUserCommand(id));

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["Success"] = "کاربر حذف شد.";
        return RedirectToAction(nameof(Index));
    }

    // ─── تغییر رمز ─── (موجود از گام قبل)
    [HttpGet]
    public async Task<IActionResult> ResetPassword(Guid id)
    {
        var result = await Mediator.Send(new GetUserDetailsQuery(id));
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction(nameof(Index));
        }

        ViewData["Title"] = "تغییر رمز کاربر";

        return View(new ResetPasswordViewModel
        {
            UserId = result.Value!.Id,
            UserFullName = result.Value.FullName
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
    {
        ViewData["Title"] = "تغییر رمز کاربر";

        if (!ModelState.IsValid)
            return View(vm);

        var result = await Mediator.Send(
            new ResetUserPasswordCommand(vm.UserId, vm.NewPassword));

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error!);
            return View(vm);
        }

        TempData["Success"] = "رمز عبور تغییر یافت.";
        return RedirectToAction(nameof(Details), new { id = vm.UserId });
    }

    // ─── تغییر نقش ───
    [HttpGet]
    public async Task<IActionResult> ChangeRole(Guid id)
    {
        var result = await Mediator.Send(new GetUserDetailsQuery(id));
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction(nameof(Index));
        }

        var u = result.Value!;
        var currentRole = u.Roles.FirstOrDefault() ?? "";

        ViewData["Title"] = $"تغییر نقش {u.FullName}";

        return View(new ChangeRoleViewModel
        {
            UserId = u.Id,
            UserFullName = u.FullName,
            CurrentRole = currentRole,
            NewRole = currentRole,
            RoleOptions = BuildRoleOptions(currentRole)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeRole(ChangeRoleViewModel vm)
    {
        ViewData["Title"] = "تغییر نقش";

        if (!ModelState.IsValid)
        {
            vm.RoleOptions = BuildRoleOptions(vm.CurrentRole);
            return View(vm);
        }

        var result = await Mediator.Send(
            new ChangeUserRoleCommand(vm.UserId, vm.NewRole));

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error!);
            vm.RoleOptions = BuildRoleOptions(vm.CurrentRole);
            return View(vm);
        }

        TempData["Success"] = "نقش کاربر تغییر یافت.";
        return RedirectToAction(nameof(Details), new { id = vm.UserId });
    }

    // ─── Helper ───
    private static List<SelectListItem> BuildRoleOptions(string? selected)
    {
        var roles = new[]
        {
            (Value: "", Label: "همه نقش‌ها"),
            (Value: "Admin", Label: "مدیر"),
            (Value: "Teacher", Label: "استاد"),
            (Value: "Parent", Label: "والد"),
            (Value: "Student", Label: "شاگرد")
        };

        return roles.Select(r => new SelectListItem
        {
            Value = r.Value,
            Text = r.Label,
            Selected = r.Value == selected
        }).ToList();
    }
}