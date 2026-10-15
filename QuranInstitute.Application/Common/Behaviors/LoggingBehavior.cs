using MediatR;
using Microsoft.Extensions.Logging;

namespace QuranInstitute.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        => _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var name = typeof(TRequest).Name;
        _logger.LogInformation("▶ Handling {Name}", name);

        var sw = System.Diagnostics.Stopwatch.StartNew();
        var response = await next();
        sw.Stop();

        _logger.LogInformation("✔ Handled {Name} in {Elapsed} ms", name, sw.ElapsedMilliseconds);

        if (sw.ElapsedMilliseconds > 500)
            _logger.LogWarning("⚠ Slow request: {Name} ({Elapsed} ms)", name, sw.ElapsedMilliseconds);

        return response;
    }
}