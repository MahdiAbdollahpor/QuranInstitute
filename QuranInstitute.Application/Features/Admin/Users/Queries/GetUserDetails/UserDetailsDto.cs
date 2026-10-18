namespace QuranInstitute.Application.Features.Admin.Users.Queries.GetUserDetails;

public class UserDetailsDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string? NationalCode { get; set; }
    public string? Email { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public List<string> Roles { get; set; } = new();

    public List<LinkedStudentInfo> LinkedStudents { get; set; } = new();
}

public class LinkedStudentInfo
{
    public Guid StudentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
    public string ClassName { get; set; } = "—";
    public bool Active { get; set; }
}