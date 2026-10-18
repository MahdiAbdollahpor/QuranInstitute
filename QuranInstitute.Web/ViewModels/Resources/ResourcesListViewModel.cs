using QuranInstitute.Application.Features.Resources.Queries.GetResourcesByClass;

namespace QuranInstitute.Web.ViewModels.Resources;

public class ResourcesListViewModel
{
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public List<ResourceListItemDto> Resources { get; set; } = new();
}