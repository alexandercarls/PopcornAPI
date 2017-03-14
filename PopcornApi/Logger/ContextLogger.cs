using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace PopcornApi.Logger
{
    /// <summary>
    /// Context logger
    /// </summary>
    public class ContextLogger : ILogger
    {
        /// <summary>
        /// Filter
        /// </summary>
        private readonly Func<LogLevel, bool> _filter;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="filter"></param>
        public ContextLogger(string categoryName, Func<LogLevel, bool> filter)
        {
            _filter = filter;
        }

        /// <summary>
        /// Log
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="logLevel"></param>
        /// <param name="eventId"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            message = $"{logLevel}: {message}";

            Debug.WriteLine(message);
            if (exception != null)
            {
                message += Environment.NewLine + Environment.NewLine + exception;
            }
        }

        /// <summary>
        /// If logging is enabled
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return _filter == null || _filter(logLevel);
        }

        /// <summary>
        /// Not important
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="state"></param>
        /// <returns></returns>
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}
