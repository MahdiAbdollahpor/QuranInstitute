using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranInstitute.Application.Features.UserPanel.Queries.GetMyDashboard;
using QuranInstitute.Web.ViewModels.UserPanel;

namespace QuranInstitute.Web.Areas.User.Controllers;

[Area("User")]
[Authorize]
public class DashboardController : Controller
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator) => _mediator = mediator;

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "داشبورد";

        var result = await _mediator.Send(new GetMyDashboardQuery());

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
            return View(new UserDashboardViewModel());
        }

        return View(new UserDashboardViewModel { Data = result.Value! });
    }
}