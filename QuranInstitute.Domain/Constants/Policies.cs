namespace QuranInstitute.Domain.Constants;

public static class Policies
{
    public const string RequireAdmin = "RequireAdmin";
    public const string RequireStaff = "RequireStaff";   // Admin + Teacher
    public const string RequireStudent = "RequireStudent";
}