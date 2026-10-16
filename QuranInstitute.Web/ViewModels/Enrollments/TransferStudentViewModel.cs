using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuranInstitute.Web.ViewModels.Enrollments;

public class TransferStudentViewModel
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;

    public Guid FromClassId { get; set; }
    public string FromClassName { get; set; } = string.Empty;

    public Guid? ToClassId { get; set; }

    public List<SelectListItem> AvailableClasses { get; set; } = new();
}