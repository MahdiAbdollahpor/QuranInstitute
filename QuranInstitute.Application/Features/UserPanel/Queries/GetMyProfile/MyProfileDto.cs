namespace QuranInstitute.Application.Features.UserPanel.Queries.GetMyProfile;

public class MyProfileDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;

    public List<MyStudentInfo> Students { get; set; } = new();
}

public class MyStudentInfo
{
    public Guid StudentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
    public string ClassName { get; set; } = "—";
    public decimal MonthlyFee { get; set; }
    public DateTime StartDate { get; set; }
}