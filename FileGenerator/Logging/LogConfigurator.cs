using NLog;
using NLog.Config;
using NLog.Targets;

namespace FileGenerator.Logging
{
    /// <summary>
    /// Service class that configures logs
    /// </summary>
    public static class LogConfigurator
    {
        private const string Layout = @"${date:format=HH\:mm\:ss\:ms}|${level}|TID=${threadid}:${threadname}|${message} ${exception:format=ToString}";
        
        /// <summary>
        /// Configure logging using NLog
        /// </summary>
        public static void Configure()
        {
            var config = new LoggingConfiguration();

            var fileTarget = new FileTarget("logFile")
            {
                FileName = "${basedir}/logs/${longdate:cached=true}_file_generator.log",
                Layout = Layout,
                MaxArchiveFiles = 20,
                ArchiveFileName = "${basedir}/logs/${shortdate}_file_generator.{#}.log",
                ArchiveEvery = FileArchivePeriod.Day,
                ArchiveAboveSize = 5242880,
                ArchiveNumbering = ArchiveNumberingMode.Rolling
            };
            
            var consoleTarget = new ColoredConsoleTarget("logConsole")
            {
                Layout = Layout
            };
            
            config.AddRule(LogLevel.Info, LogLevel.Fatal, consoleTarget);
            config.AddRule(LogLevel.Info, LogLevel.Fatal, fileTarget);
            
            LogManager.Configuration = config;
        }
    }
}