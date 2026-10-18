using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Features.Admin.Users.Commands.ResetUserPassword;
using QuranInstitute.Domain.Constants;
using QuranInstitute.Domain.Entities.Users;
using QuranInstitute.Web.ViewModels.Admin;

namespace QuranInstitute.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
public class UsersController : Controller
{
    private readonly IMediator _mediator;
    private readonly UserManager<AppUser> _userMgr;

    public UsersController(IMediator mediator, UserManager<AppUser> userMgr)
    {
        _mediator = mediator;
        _userMgr = userMgr;
    }

    // ─── لیست کاربران ───
    [HttpGet]
    public async Task<IActionResult> Index(string? search = null)
    {
        ViewData["Title"] = "کاربران";

        var query = _userMgr.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(u =>
                u.FullName.Contains(s) ||
                (u.UserName != null && u.UserName.Contains(s)) ||
                (u.PhoneNumber != null && u.PhoneNumber.Contains(s)));
        }

        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Take(200)
            .ToListAsync();

        // نقش‌ها
        var roleMap = new Dictionary<Guid, List<string>>();
        foreach (var u in users)
        {
            var roles = await _userMgr.GetRolesAsync(u);
            roleMap[u.Id] = roles.ToList();
        }

        ViewBag.RoleMap = roleMap;
        ViewBag.Search = search;

        return View(users);
    }

    // ─── تغییر رمز ───
    [HttpGet]
    public async Task<IActionResult> ResetPassword(Guid id)
    {
        var user = await _userMgr.FindByIdAsync(id.ToString());
        if (user is null)
        {
            TempData["Error"] = "کاربر یافت نشد.";
            return RedirectToAction(nameof(Index));
        }

        ViewData["Title"] = "تغییر رمز کاربر";

        return View(new ResetPasswordViewModel
        {
            UserId = user.Id,
            UserFullName = user.FullName
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
    {
        ViewData["Title"] = "تغییر رمز کاربر";

        if (!ModelState.IsValid)
            return View(vm);

        var result = await _mediator.Send(
            new ResetUserPasswordCommand(vm.UserId, vm.NewPassword));

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error!);
            return View(vm);
        }

        TempData["Success"] = "رمز عبور کاربر با موفقیت تغییر کرد.";
        return RedirectToAction(nameof(Index));
    }
}