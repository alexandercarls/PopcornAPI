using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PopcornApi.Database;

namespace PopcornApi.Logger
{
    /// <summary>
    /// Context logger provider
    /// </summary>
    public class ContextLoggerProvider : ILoggerProvider
    {
        private readonly Func<LogLevel, bool> _filter;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filter"></param>
        public ContextLoggerProvider(Func<LogLevel, bool> filter)
        {
            _filter = filter;
        }

        /// <summary>
        /// Returns a <see cref="ContextLogger"/>
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public ILogger CreateLogger(string categoryName)
        {
            return new ContextLogger(categoryName, _filter);
        }

        public void Dispose()
        {
        }
    }
}
