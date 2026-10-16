using QuranInstitute.Application.Features.Enrollments.Queries.GetClassStudents;

namespace QuranInstitute.Web.ViewModels.Enrollments;

public class ClassStudentsViewModel
{
    public ClassStudentsResult Data { get; set; } = new();
}