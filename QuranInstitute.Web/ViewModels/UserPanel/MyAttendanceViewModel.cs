using QuranInstitute.Application.Features.UserPanel.Queries.GetMyAttendance;

namespace QuranInstitute.Web.ViewModels.UserPanel;

public class MyAttendanceViewModel
{
    public MyAttendanceDto Data { get; set; } = new();
    public Guid StudentId { get; set; }
}