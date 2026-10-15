using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QuranInstitute.Web.Areas.User.Controllers;

[Area("User")]
[Authorize]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "داشبورد";
        return View();
    }
}