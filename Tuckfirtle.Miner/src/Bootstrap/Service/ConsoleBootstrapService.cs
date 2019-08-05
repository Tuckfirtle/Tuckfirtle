// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Reflection;
using System.Runtime.Versioning;
using TheDialgaTeam.Core.DependencyInjection.Service;
using TheDialgaTeam.Core.Logger;
using Tuckfirtle.Miner.Config.Model;

namespace Tuckfirtle.Miner.Bootstrap.Service
{
    internal sealed class ConsoleBootstrapService : IServiceExecutor
    {
        private IConsoleLogger ConsoleLogger { get; }

        private IConfig Config { get; }

        public ConsoleBootstrapService(IConsoleLogger consoleLogger, IConfig config)
        {
            ConsoleLogger = consoleLogger;
            Config = config;
        }

        public void Execute()
        {
            var config = Config;

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var frameworkVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<TargetFrameworkAttribute>().FrameworkName;

            var consoleMessageBuilder = new ConsoleMessageBuilder();

            consoleMessageBuilder.Write(" * ", ConsoleColor.Green, false)
                .Write("ABOUT".PadRight(13), false)
                .Write($"Tuckfirtle Miner/{version} ", ConsoleColor.Cyan, false)
                .WriteLine(frameworkVersion, false);

            consoleMessageBuilder.Write(" * ", ConsoleColor.Green, false)
                .Write("THREADS".PadRight(13), false)
                .Write($"{config.Threads.Length}", ConsoleColor.Cyan, false)
                .Write(", ", false)
                .Write("TuckfirtlePow", false)
                .Write(", ", false)
                .WriteLine($"donate={config.DonateLevel}%", config.DonateLevel == 0 ? ConsoleColor.Red : ConsoleColor.White, false);

            consoleMessageBuilder.Write(" * ", ConsoleColor.Green, false)
                .Write("COMMANDS".PadRight(13), false)
                .Write("h", ConsoleColor.Magenta, false)
                .WriteLine("ashrate", false);

            ConsoleLogger.LogMessage(consoleMessageBuilder.Build());
        }
    }
}