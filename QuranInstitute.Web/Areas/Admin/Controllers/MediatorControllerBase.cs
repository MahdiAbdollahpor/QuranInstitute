using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace QuranInstitute.Web.Areas.Admin.Controllers;

public abstract class MediatorControllerBase : Controller
{
    protected readonly IMediator Mediator;

    protected MediatorControllerBase(IMediator mediator) => Mediator = mediator;
}