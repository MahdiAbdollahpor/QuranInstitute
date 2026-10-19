using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Notifications.Queries.GetAnnouncementsList;

public class GetAnnouncementsListQuery : IRequest<PagedList<AnnouncementListItemDto>>
{
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}