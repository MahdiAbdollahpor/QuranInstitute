using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranInstitute.Application.Features.Enrollments.Queries.GetPendingEnrollments;
using QuranInstitute.Domain.Constants;

namespace QuranInstitute.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Teacher}")]
public class DashboardController : MediatorControllerBase
{
    public DashboardController(IMediator mediator) : base(mediator) { }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "داشبورد";

        var pending = await Mediator.Send(new GetPendingEnrollmentsQuery());
        ViewBag.PendingCount = pending.Count;

        return View();
    }
}