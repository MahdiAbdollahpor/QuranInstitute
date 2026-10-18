using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Students.Commands.CreateStudentAccount;

public class CreateStudentAccountCommand : IRequest<Result>
{
    public Guid StudentId { get; set; }
    public string Password { get; set; } = string.Empty;

    // ★ Role حذف شد — همیشه Parent است
    public CreateStudentAccountCommand(Guid studentId, string password)
    {
        StudentId = studentId;
        Password = password;
    }
}