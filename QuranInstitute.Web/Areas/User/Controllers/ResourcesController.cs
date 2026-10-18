using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranInstitute.Application.Features.Resources.Queries.GetResourcesByClass;
using QuranInstitute.Application.Features.UserPanel.Queries.GetMyClasses;
using QuranInstitute.Web.ViewModels.Resources;

namespace QuranInstitute.Web.Areas.User.Controllers;

[Area("User")]
[Authorize]
public class ResourcesController : Controller
{
    private readonly IMediator _mediator;

    public ResourcesController(IMediator mediator) => _mediator = mediator;

    public async Task<IActionResult> Index(Guid classId)
    {
        // بررسی دسترسی
        var myClasses = await _mediator.Send(new GetMyClassesQuery());
        if (!myClasses.IsSuccess ||
            !myClasses.Value!.Any(c => c.ClassId == classId))
        {
            TempData["Error"] = "شما در این کلاس ثبت‌نام نکرده‌اید.";
            return RedirectToAction("Index", "Classes");
        }

        var cls = myClasses.Value!.First(c => c.ClassId == classId);
        var result = await _mediator.Send(new GetResourcesByClassQuery(classId));

        ViewData["Title"] = $"فایل‌های کلاس {cls.ClassName}";
        return View(new ResourcesListViewModel
        {
            ClassId = classId,
            ClassName = cls.ClassName,
            Resources = result.IsSuccess ? result.Value! : new()
        });
    }
}