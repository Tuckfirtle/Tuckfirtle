using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TheDialgaTeam.Core.Logger.Console;
using TheDialgaTeam.Microsoft.Extensions.DependencyInjection;
using Tuckfirtle.Node.Services.Bootstrap;
using Tuckfirtle.Node.Services.Console;
using Tuckfirtle.Node.Services.IO;

namespace Tuckfirtle.Node
{
    public static class Program
    {
        /// <summary>
        /// This is the program cancellation token source to properly signal the application to stop.
        /// </summary>
        public static CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();

        /// <summary>
        /// List of managed tasks to wait before signalling the application to stop.
        /// </summary>
        public static List<Task> TasksToAwait { get; } = new List<Task>();

        /// <summary>
        /// A container of registered services in the application.
        /// </summary>
        public static ServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Main execution code begin here.
        /// </summary>
        public static void Main()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddInterfacesAndSelfAsSingleton<FilePathService>();
            serviceCollection.AddInterfacesAndSelfAsSingleton<LoggerService>();
            serviceCollection.AddInterfacesAndSelfAsSingleton<BootstrapService>();

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var loggerService = ServiceProvider.GetService<LoggerService>();

            try
            {
                ServiceProvider.InitializeServices();
                ServiceProvider.LateInitializeServices();

                Task.WaitAll(TasksToAwait.ToArray());

                ServiceProvider.DisposeServices();
                Dispose();

                Environment.Exit(0);
            }
            catch (AggregateException ex)
            {
                var consoleMessages = new ConsoleMessageBuilder();

                foreach (var exInnerException in ex.InnerExceptions)
                {
                    if (exInnerException is OperationCanceledException)
                        continue;

                    consoleMessages.WriteLine(exInnerException.ToString(), ConsoleColor.Red);
                }

                var message = consoleMessages.WriteLine("Press Enter/Return to exit...").Build();

                if (message.Length > 1)
                {
                    loggerService.LogMessage(message);
                    Console.ReadLine();
                }

                ExitWithFault();
            }
            catch (Exception ex)
            {
                loggerService.LogMessage(new ConsoleMessageBuilder()
                    .WriteLine(ex.ToString(), ConsoleColor.Red)
                    .WriteLine("Press Enter/Return to exit...")
                    .Build());

                Console.ReadLine();

                ExitWithFault();
            }
        }

        private static void ExitWithFault()
        {
            CancellationTokenSource?.Cancel();
            ServiceProvider?.DisposeServices();
            Dispose();

            Environment.Exit(1);
        }

        private static void Dispose()
        {
            CancellationTokenSource?.Dispose();
            ServiceProvider?.Dispose();
        }
    }
}