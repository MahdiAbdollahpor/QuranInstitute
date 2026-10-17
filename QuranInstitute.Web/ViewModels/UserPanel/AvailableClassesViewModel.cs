using QuranInstitute.Application.Features.UserPanel.Queries.GetAvailableClasses;

namespace QuranInstitute.Web.ViewModels.UserPanel;

public class AvailableClassesViewModel
{
    public List<AvailableClassDto> Classes { get; set; } = new();
    public Guid? SelectedStudentId { get; set; }
}