using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using QuranInstitute.Application.Common.Interfaces;

namespace QuranInstitute.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _http;

    public CurrentUserService(IHttpContextAccessor http) => _http = http;

    public Guid? UserId
    {
        get
        {
            var v = _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(v, out var id) ? id : null;
        }
    }

    public string? UserName => _http.HttpContext?.User?.Identity?.Name;
    public bool IsAuthenticated => _http.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    public bool IsInRole(string role) => _http.HttpContext?.User?.IsInRole(role) ?? false;
}