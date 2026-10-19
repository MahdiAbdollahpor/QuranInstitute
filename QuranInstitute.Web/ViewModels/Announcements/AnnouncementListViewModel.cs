using QuranInstitute.Application.Common.Models;
using QuranInstitute.Application.Features.Notifications.Queries.GetAnnouncementsList;

namespace QuranInstitute.Web.ViewModels.Announcements;

public class AnnouncementListViewModel
{
    public PagedList<AnnouncementListItemDto> Announcements { get; set; } = new();
    public string? Search { get; set; }
}