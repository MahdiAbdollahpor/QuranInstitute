using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
}