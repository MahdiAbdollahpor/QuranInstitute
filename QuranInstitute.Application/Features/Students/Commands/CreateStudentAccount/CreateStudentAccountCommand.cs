using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Students.Commands.CreateStudentAccount;

public class CreateStudentAccountCommand : IRequest<Result>
{
    public Guid StudentId { get; set; }
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Parent";  // Parent | Student

    public CreateStudentAccountCommand(Guid studentId, string password, string role = "Parent")
    {
        StudentId = studentId;
        Password = password;
        Role = role;
    }
}