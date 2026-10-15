using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuranInstitute.Domain.Constants;
using QuranInstitute.Domain.Entities.Users;
using QuranInstitute.Web.ViewModels.Account;

namespace QuranInstitute.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<AppUser> _signIn;
    private readonly UserManager<AppUser> _userMgr;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        SignInManager<AppUser> signIn,
        UserManager<AppUser> userMgr,
        ILogger<AccountController> logger)
    {
        _signIn = signIn;
        _userMgr = userMgr;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectBasedOnRole();

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = await _userMgr.FindByNameAsync(vm.Mobile);
        if (user is null)
        {
            ModelState.AddModelError("", "شماره موبایل یا رمز عبور اشتباه است.");
            return View(vm);
        }

        if (!user.IsActive)
        {
            ModelState.AddModelError("", "حساب کاربری شما غیرفعال است. با مدیر تماس بگیرید.");
            return View(vm);
        }

        var result = await _signIn.PasswordSignInAsync(
            user, vm.Password, vm.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            user.LastLoginAt = DateTime.UtcNow;
            await _userMgr.UpdateAsync(user);

            _logger.LogInformation("کاربر {Name} وارد شد.", user.FullName);

            if (!string.IsNullOrEmpty(vm.ReturnUrl) && Url.IsLocalUrl(vm.ReturnUrl))
                return Redirect(vm.ReturnUrl);

            return RedirectBasedOnRole();
        }

        if (result.IsLockedOut)
            ModelState.AddModelError("", "حساب شما به‌دلیل تلاش‌های ناموفق قفل شده است.");
        else
            ModelState.AddModelError("", "شماره موبایل یا رمز عبور اشتباه است.");

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signIn.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied() => View();

    // ─────────────────────────────────────────────
    private IActionResult RedirectBasedOnRole()
    {
        if (User.IsInRole(Roles.Admin) || User.IsInRole(Roles.Teacher))
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

        return RedirectToAction("Index", "Dashboard", new { area = "User" });
    }
}