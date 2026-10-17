using Microsoft.AspNetCore.Mvc.Rendering;
using QuranInstitute.Application.Features.Attendances.Queries.GetAttendanceByClassDate;

namespace QuranInstitute.Web.ViewModels.Attendances;

public class AttendanceFormViewModel
{
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public int[] Days { get; set; } = Array.Empty<int>();
    public DateTime Date { get; set; }
    public bool IsClassDay { get; set; }
    public bool AlreadyRecorded { get; set; }

    public List<AttendanceEntryDto> Entries { get; set; } = new();

    public List<SelectListItem> Classes { get; set; } = new();
}