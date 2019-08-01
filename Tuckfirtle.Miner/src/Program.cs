using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using TheDialgaTeam.Core.DependencyInjection;
using TheDialgaTeam.Core.Logger;
using TheDialgaTeam.Core.Logger.DependencyInjection;
using Tuckfirtle.Miner.Config.Json;

namespace Tuckfirtle.Miner
{
    public static class Program
    {
        private static DependencyManager DependencyManager { get; } = new DependencyManager();

        /// <summary>
        /// Main execution code begin here.
        /// </summary>
        public static void Main()
        {
            DependencyManager.InstallFactory(new ConsoleStreamQueuedTaskLoggerFactoryInstaller());
            DependencyManager.InstallFactory(new JsonConfigFactoryInstaller(Path.Combine(Environment.CurrentDirectory, "config.json")));

            DependencyManager.InstallServices(collection => { collection.AddSingleton<ProgramBootstrap>(); });

            DependencyManager.BuildAndExecute(provider =>
            {
                var programBootstrap = provider.GetRequiredService<ProgramBootstrap>();
                programBootstrap.Execute();
            }, (provider, exception) =>
            {
                var consoleLogger = provider.GetRequiredService<IConsoleLogger>() ?? new ConsoleStreamLogger(Console.Error);

                if (exception is AggregateException aggregateException)
                {
                    var consoleMessages = new ConsoleMessageBuilder();

                    foreach (var exInnerException in aggregateException.InnerExceptions)
                    {
                        if (exInnerException is OperationCanceledException)
                            continue;

                        consoleMessages.WriteLine(exInnerException.ToString(), ConsoleColor.Red);
                    }

                    var message = consoleMessages.WriteLine("Press Enter/Return to exit...").Build();

                    if (message.Length > 1)
                    {
                        consoleLogger.LogMessage(message);
                        Console.ReadLine();
                    }

                    ExitWithFault();
                }
                else
                {
                    consoleLogger.LogMessage(new ConsoleMessageBuilder()
                        .WriteLine(exception.ToString(), ConsoleColor.Red)
                        .WriteLine("Press Enter/Return to exit...")
                        .Build());

                    Console.ReadLine();

                    ExitWithFault();
                }
            });
        }

        private static void ExitWithFault()
        {
            Dispose();
            Environment.Exit(1);
        }

        private static void Dispose()
        {
            DependencyManager.Dispose();
        }
    }
}