using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog.Events;
using Serilog;

namespace InventoryManagementSystemUI
{
    public class LoggerConfigurator : ILoggerConfigurator
    {
        public void ConfigureSeriLogs()
        {
            // Load appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // or AppDomain.CurrentDomain.BaseDirectory
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var serilogSection = configuration.GetSection("Logging:SeriLog");
            var logDirectory = serilogSection["LogDirectory"];
            var eventSourceName = serilogSection["EventSourceName"];
            var successLogFileName = serilogSection["SuccessLogFileName"];
            var errorLogFileName = serilogSection["ErrorLogFileName"];

            if (string.IsNullOrEmpty(logDirectory) || string.IsNullOrEmpty(eventSourceName)
                || string.IsNullOrEmpty(successLogFileName) || string.IsNullOrEmpty(errorLogFileName))
            {
                throw new InvalidOperationException("One or more SeriLog configuration values are missing.");
            }

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information)
                    .WriteTo.File(Path.Combine(logDirectory, successLogFileName), rollingInterval: RollingInterval.Day))
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error)
                    .WriteTo.File(Path.Combine(logDirectory, errorLogFileName), rollingInterval: RollingInterval.Day));

#if WINDOWS
            loggerConfiguration = loggerConfiguration.WriteTo.EventLog(eventSourceName, restrictedToMinimumLevel: LogEventLevel.Information);
#endif

            Log.Logger = loggerConfiguration.CreateLogger();
        }
    }
}
