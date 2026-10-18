namespace QuranInstitute.Application.Features.Admin.Users.Queries.GetUsersList;

public class UserListItemDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string? NationalCode { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public List<string> Roles { get; set; } = new();

    /// <summary>تعداد شاگردانی که به این کاربر وصل هستند</summary>
    public int LinkedStudentsCount { get; set; }
}