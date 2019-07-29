using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TheDialgaTeam.Core.Logger;
using TheDialgaTeam.Core.Logger.DependencyInjection;

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
            // Create necessary folders before initialize.
            var logDirectory = Path.Combine(Environment.CurrentDirectory, "Logs");

            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            // Dependency injection container. 
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddConsoleStreamWriteToFileQueuedTaskLoggerService(Console.Out, Path.Combine(logDirectory, $"{DateTime.Now:yyyy-MM-dd}.log"), CancellationTokenSource.Token);
            serviceCollection.AddSingleton<Test>();

            ServiceProvider = serviceCollection.BuildServiceProvider();

            // Insert task that should be awaited to prevent shutdown.
            var consoleLogger = ServiceProvider.GetRequiredService<ConsoleStreamWriteToFileQueuedTaskLogger>();
            TasksToAwait.Add(consoleLogger.ConsoleMessageQueueTask);

            // Main execution code starts below!
            try
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                var frameworkVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<TargetFrameworkAttribute>().FrameworkName;
                Console.Title = $"Tuckfirtle Node v{version} ({frameworkVersion})";

                consoleLogger.LogMessage($"Initializing Tuckfirtle Node v{version}...");
                ServiceProvider.GetService<Test>().Start();

                Task.WaitAll(TasksToAwait.ToArray());

                var disposableServices = ServiceProvider.GetServices<IDisposable>().Reverse();

                foreach (var disposableService in disposableServices)
                    disposableService.Dispose();

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
                    consoleLogger.LogMessage(message);
                    Console.ReadLine();
                }

                ExitWithFault();
            }
            catch (Exception ex)
            {
                consoleLogger.LogMessage(new ConsoleMessageBuilder()
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

            var disposableServices = ServiceProvider.GetServices<IDisposable>().Reverse();

            foreach (var disposableService in disposableServices)
                disposableService.Dispose();
            
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