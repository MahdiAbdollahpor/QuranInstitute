using QuranInstitute.Application.Features.UserPanel.Queries.GetMyClasses;

namespace QuranInstitute.Web.ViewModels.UserPanel;

public class MyClassesViewModel
{
    public List<MyClassDto> Classes { get; set; } = new();
}