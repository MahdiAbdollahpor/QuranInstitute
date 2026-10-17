using MediatR;

namespace QuranInstitute.Application.Features.Attendances.Queries.GetTodayAbsentees;

public class GetTodayAbsenteesQuery : IRequest<List<TodaysAbsenteeDto>>
{
    /// <summary>اگر null باشد، از تاریخ امروز استفاده می‌شود</summary>
    public DateTime? Date { get; set; }
}