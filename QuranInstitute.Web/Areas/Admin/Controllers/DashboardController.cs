using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranInstitute.Domain.Constants;

namespace QuranInstitute.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "داشبورد";
        return View();
    }
}