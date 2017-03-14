using Microsoft.ApplicationInsights;

namespace PopcornApi.Services.Logging
{
    /// <summary>
    /// The logger using Application Insights
    /// </summary>
    public class LoggingService : ILoggingService
    {
        public TelemetryClient Telemetry { get; }

        /// <summary>
        /// Create an instance of <see cref="LoggingService"/>
        /// </summary>
        /// <param name="connectionString"></param>
        public LoggingService(string connectionString)
        {
            Telemetry = new TelemetryClient(new Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration
            {
                InstrumentationKey = connectionString
            });
        }
    }
}