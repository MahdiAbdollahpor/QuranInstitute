using MediatR;
using Microsoft.EntityFrameworkCore;
using QuranInstitute.Application.Common.Interfaces;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Students.Commands.DeactivateStudent;

public class DeactivateStudentCommandHandler
    : IRequestHandler<DeactivateStudentCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public DeactivateStudentCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(DeactivateStudentCommand request, CancellationToken ct)
    {
        var student = await _db.Students
            .FirstOrDefaultAsync(s => s.Id == request.Id, ct);

        if (student is null)
            return Result.Failure("شاگرد یافت نشد.");

        student.Active = false;
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}