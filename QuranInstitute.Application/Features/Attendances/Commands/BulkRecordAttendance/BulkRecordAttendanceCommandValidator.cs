using FluentValidation;

namespace QuranInstitute.Application.Features.Attendances.Commands.BulkRecordAttendance;

public class BulkRecordAttendanceCommandValidator : AbstractValidator<BulkRecordAttendanceCommand>
{
    public BulkRecordAttendanceCommandValidator()
    {
        RuleFor(x => x.ClassId).NotEmpty();
        RuleFor(x => x.Date).NotEmpty();

        RuleFor(x => x.Items)
            .NotNull().WithMessage("لیست حضور و غیاب خالی است.")
            .Must(i => i.Count > 0).WithMessage("هیچ رکوردی برای ثبت وجود ندارد.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.StudentId).NotEmpty();
            item.RuleFor(i => i.Status).IsInEnum();
        });
    }
}