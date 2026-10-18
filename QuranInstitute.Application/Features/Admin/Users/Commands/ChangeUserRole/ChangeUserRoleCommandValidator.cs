using FluentValidation;

namespace QuranInstitute.Application.Features.Admin.Users.Commands.ChangeUserRole;

public class ChangeUserRoleCommandValidator
    : AbstractValidator<ChangeUserRoleCommand>
{
    private static readonly string[] AllowedRoles =
        { "Admin", "Teacher", "Parent", "Student" };

    public ChangeUserRoleCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.NewRole)
            .NotEmpty()
            .Must(r => AllowedRoles.Contains(r))
            .WithMessage("نقش معتبر نیست.");
    }
}