using System;
using System.IO;
using TheDialgaTeam.Core.Logger.Console;
using TheDialgaTeam.Microsoft.Extensions.DependencyInjection;
using Tuckfirtle.Node.Services.IO;

namespace Tuckfirtle.Node.Services.Console
{
    public sealed class LoggerService : IInitializable, IConsoleLogger, IDisposable
    {
        private FilePathService FilePathService { get; }

        private ConsoleStreamWriteToFileQueuedTaskLogger InternalLogger { get; set; }

        public LoggerService(FilePathService filePathService)
        {
            FilePathService = filePathService;
        }

        public void Initialize()
        {
            InternalLogger = new ConsoleStreamWriteToFileQueuedTaskLogger(System.Console.Out, new StreamWriter(new FileStream(FilePathService.LoggerFilePath, FileMode.Append, FileAccess.Write, FileShare.Read)), Program.CancellationTokenSource.Token);
            Program.TasksToAwait.Add(InternalLogger.QueuedTaskLoggerTask);
        }

        public void LogMessage(string message)
        {
            InternalLogger.LogMessage(message);
        }

        public void LogMessage(string message, ConsoleColor color)
        {
            InternalLogger.LogMessage(message, color);
        }

        public void LogMessage(string message, bool includeDateTime)
        {
            InternalLogger.LogMessage(message, includeDateTime);
        }

        public void LogMessage(string message, ConsoleColor color, bool includeDateTime)
        {
            InternalLogger.LogMessage(message, color, includeDateTime);
        }

        public void LogMessage(ConsoleMessage[] consoleMessages)
        {
            InternalLogger.LogMessage(consoleMessages);
        }

        public void Dispose()
        {
            InternalLogger?.Dispose();
        }
    }
}