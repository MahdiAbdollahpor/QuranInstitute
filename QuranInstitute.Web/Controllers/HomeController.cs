using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranInstitute.Domain.Constants;

namespace QuranInstitute.Web.Controllers;

public class HomeController : Controller
{
    [AllowAnonymous]
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            if (User.IsInRole(Roles.Admin) || User.IsInRole(Roles.Teacher))
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            return RedirectToAction("Index", "Dashboard", new { area = "User" });
        }

        // فعلاً صفحه لاگین
        return RedirectToAction("Login", "Account");
    }

    [AllowAnonymous]
    public IActionResult Error() => View();
}