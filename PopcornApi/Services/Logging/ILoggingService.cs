using Microsoft.ApplicationInsights;

namespace PopcornApi.Services.Logging
{
    /// <summary>
    /// The logger using Application Insights
    /// </summary>
    public interface ILoggingService
    {
        TelemetryClient Telemetry { get; }
    }
}
