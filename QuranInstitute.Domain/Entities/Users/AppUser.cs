using Microsoft.AspNetCore.Identity;
using QuranInstitute.Domain.Entities.Education;

namespace QuranInstitute.Domain.Entities.Users;

public class AppUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public string? NationalCode { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }

    // Navigation
    public ICollection<Student> Students { get; set; } = new List<Student>();
}