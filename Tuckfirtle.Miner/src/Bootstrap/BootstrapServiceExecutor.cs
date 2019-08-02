using System;
using System.Reflection;
using System.Runtime.Versioning;
using TheDialgaTeam.Core.DependencyInjection.Service;

namespace Tuckfirtle.Miner.Bootstrap
{
    public sealed class BootstrapServiceExecutor : IServiceExecutor
    {
        public void Execute()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var frameworkVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<TargetFrameworkAttribute>().FrameworkName;
            Console.Title = $"Tuckfirtle Miner v{version} ({frameworkVersion})";
        }
    }
}