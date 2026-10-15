using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace QuranInstitute.Infrastructure.Persistence.Context;

public class ApplicationDbContextFactory
    : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var basePath = ResolveWebProjectPath();

        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
            .Build();

        var cs = config.GetConnectionString("DefaultConnection")
                 ?? throw new InvalidOperationException(
                     "Connection string 'DefaultConnection' در appsettings.json پروژه Web پیدا نشد.");

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(cs, sql =>
                sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
            .Options;

        return new ApplicationDbContext(options);
    }

    private static string ResolveWebProjectPath()
    {
        var current = Directory.GetCurrentDirectory();

        if (File.Exists(Path.Combine(current, "appsettings.json")))
            return current;

        var candidates = new[]
        {
            Path.Combine(current, "..", "QuranInstitute.Web"),
            Path.Combine(current, "..", "..", "QuranInstitute.Web"),
            Path.Combine(current, "..", "..", "src", "QuranInstitute.Web"),
            Path.Combine(current, "..", "src", "QuranInstitute.Web")
        };

        foreach (var c in candidates)
        {
            var full = Path.GetFullPath(c);
            if (Directory.Exists(full) &&
                File.Exists(Path.Combine(full, "appsettings.json")))
                return full;
        }

        var dir = new DirectoryInfo(current);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, "src", "QuranInstitute.Web");
            if (Directory.Exists(candidate) &&
                File.Exists(Path.Combine(candidate, "appsettings.json")))
                return candidate;

            dir = dir.Parent;
        }

        throw new InvalidOperationException(
            "مسیر پروژه Web برای خواندن appsettings.json پیدا نشد.");
    }
}