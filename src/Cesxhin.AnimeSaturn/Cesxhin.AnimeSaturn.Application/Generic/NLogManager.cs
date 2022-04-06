using NLog;
using NLog.Config;
using NLog.Targets;

namespace Cesxhin.AnimeSaturn.Application.Generic
{
    public class NLogManager
    {
        public static void Configure(LogLevel minLevel)
        {
            var config = new LoggingConfiguration();

            var consoleTarget = new ConsoleTarget();
            config.AddTarget("console", consoleTarget);

            var rule = new LoggingRule("*", minLevel, consoleTarget);
            config.LoggingRules.Add(rule);

            LogManager.Configuration = config;
        }

        public static LogLevel GetLevel(string level)
        {
            switch (level)
            {
                case "debug":
                    return LogLevel.Debug;
                case "info":
                    return LogLevel.Info;
                case "warn":
                    return LogLevel.Warn;
                case "error":
                    return LogLevel.Error;
                case "fatal":
                    return LogLevel.Fatal;
                default:
                    return LogLevel.Info;
            }
        }
    }
}
