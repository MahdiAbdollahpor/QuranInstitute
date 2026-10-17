using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuranInstitute.Application.Features.Attendances.Queries.GetTodayAbsentees;
using QuranInstitute.Application.Features.Enrollments.Queries.GetPendingEnrollments;
using QuranInstitute.Application.Features.Tuition.Queries.GetDebtorsList;
using QuranInstitute.Application.Features.Tuition.Queries.GetMonthlyIncome;
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
        var absentees = await Mediator.Send(new GetTodayAbsenteesQuery());
        var income = await Mediator.Send(new GetMonthlyIncomeQuery());
        var debtors = await Mediator.Send(new GetDebtorsListQuery
        {
            PageSize = 5
        });

        ViewBag.PendingCount = pending.Count;
        ViewBag.Absentees = absentees;
        ViewBag.Income = income.IsSuccess ? income.Value : null;
        ViewBag.DebtorsCount = debtors.TotalCount;
        ViewBag.DebtorsSample = debtors.Items;
        ViewBag.TotalDebt = debtors.Items.Sum(x => x.Balance);

        return View();
    }
}