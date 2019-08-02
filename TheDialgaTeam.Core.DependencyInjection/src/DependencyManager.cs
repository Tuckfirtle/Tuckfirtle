using System;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace TheDialgaTeam.Core.DependencyInjection
{
    public sealed class DependencyManager : IDisposable
    {
        private ServiceCollection ServiceCollection { get; }

        private ServiceProvider ServiceProvider { get; set; }

        public DependencyManager()
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddSingleton(new CancellationTokenSource());
            ServiceCollection.AddSingleton<TaskAwaiter>();
        }

        public void InstallServices(Action<IServiceCollection> installer)
        {
            installer(ServiceCollection);
        }

        public void InstallFactory(FactoryInstaller installer)
        {
            installer.Install(ServiceCollection);
        }

        public void BuildAndExecute(Action<IServiceProvider> executeAction, Action<IServiceProvider, Exception> executeFailedAction)
        {
            try
            {
                ServiceProvider = ServiceCollection.BuildServiceProvider();

                executeAction(ServiceProvider);

                var taskAwaiter = ServiceProvider.GetRequiredService<TaskAwaiter>();
                taskAwaiter.WaitAll();

                var disposableServices = ServiceProvider.GetServices<IDisposable>().Reverse();

                foreach (var disposableService in disposableServices)
                    disposableService.Dispose();

                Dispose();

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                executeFailedAction(ServiceProvider, ex);
            }
        }

        public void Dispose()
        {
            ServiceProvider?.Dispose();
        }
    }
}