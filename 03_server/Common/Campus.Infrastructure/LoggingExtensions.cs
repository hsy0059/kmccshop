using Microsoft.Extensions.Logging;

namespace Campus.Infrastructure;

public static class LoggingExtensions
{
    public static ILoggingBuilder AddCampusLogging(this ILoggingBuilder logging)
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.SetMinimumLevel(LogLevel.Information);
        return logging;
    }

    public static IDisposable? BeginScope(this ILogger logger, string key, object value)
    {
        return logger.BeginScope(new Dictionary<string, object> { { key, value } });
    }
}
