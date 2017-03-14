using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PopcornApi.Logger;

namespace PopcornApi.Database
{
    public class PopcornContextFactory : IDbContextFactory<PopcornContext>
    {
        public PopcornContext Create(DbContextFactoryOptions options)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");
            var configuration = builder.Build();

            var optionsBuilder = new DbContextOptionsBuilder<PopcornContext>();
            optionsBuilder.UseSqlServer(configuration["SQL:ConnectionString"]);

            // Add logging
            ILoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new ContextLoggerProvider(logLevel => logLevel >= LogLevel.Information));

            optionsBuilder.UseLoggerFactory(loggerFactory);
            return new PopcornContext(optionsBuilder.Options);
        }
    }
}