using Microsoft.AspNetCore.Identity;

namespace QuranInstitute.Domain.Entities.Users;

public class AppRole : IdentityRole<Guid>
{
    public string? Description { get; set; }
    public AppRole() : base() { }
    public AppRole(string name) : base(name) { }
}