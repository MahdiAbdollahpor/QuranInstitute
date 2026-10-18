using MediatR;
using QuranInstitute.Application.Common.Models;

namespace QuranInstitute.Application.Features.Settings.Queries.GetSettings;

public class GetSettingsQuery : IRequest<Result<SettingsDto>>
{
}