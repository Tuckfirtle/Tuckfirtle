using System.Reflection;
using System.Runtime.Versioning;
using TheDialgaTeam.Microsoft.Extensions.DependencyInjection;
using Tuckfirtle.Node.Services.Console;

namespace Tuckfirtle.Node.Services.Bootstrap
{
    public sealed class BootstrapService : IInitializable
    {
        private LoggerService LoggerService { get; }

        public BootstrapService(LoggerService loggerService)
        {
            LoggerService = loggerService;
        }

        public void Initialize()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var frameworkVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<TargetFrameworkAttribute>().FrameworkName;
            System.Console.Title = $"Tuckfirtle Node v{version} ({frameworkVersion})";

            LoggerService.LogMessage($"Initializing Tuckfirtle Node v{version} ({frameworkVersion})...");
        }
    }
}