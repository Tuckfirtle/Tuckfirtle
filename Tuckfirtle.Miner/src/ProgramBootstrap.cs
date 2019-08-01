using System;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading;
using TheDialgaTeam.Core.Logger;
using Tuckfirtle.Miner.Config;

namespace Tuckfirtle.Miner
{
    public class ProgramBootstrap
    {
        private IConsoleLogger ConsoleLogger { get; }

        private IConfig Config { get; }

        private CancellationTokenSource CancellationTokenSource { get; }

        public ProgramBootstrap(IConsoleLogger consoleLogger, IConfig config, CancellationTokenSource cancellationTokenSource)
        {
            ConsoleLogger = consoleLogger;
            Config = config;
            CancellationTokenSource = cancellationTokenSource;
        }

        public void Execute()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var frameworkVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<TargetFrameworkAttribute>().FrameworkName;
            Console.Title = $"Tuckfirtle Miner v{version} ({frameworkVersion})";

            var config = Config;
            var consoleLogger = ConsoleLogger;

            if (!config.LoadConfig())
            {
                config.CreateConfig();

                consoleLogger.LogMessage(new ConsoleMessageBuilder()
                    .WriteLine($"Generated configuration file is at: \"{Path.Combine(Environment.CurrentDirectory, "config.json")}\"", false)
                    .WriteLine("Press Enter/Return to exit...", false)
                    .Build());

                Console.ReadLine();
                CancellationTokenSource.Cancel();
            }
        }
    }
}