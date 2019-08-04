using System;
using System.Reflection;
using System.Runtime.Versioning;
using TheDialgaTeam.Core.DependencyInjection.Service;
using Tuckfirtle.Core;

namespace Tuckfirtle.Node.Bootstrap.Service
{
    internal sealed class BootstrapService : IServiceExecutor
    {
        public void Execute()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var frameworkVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<TargetFrameworkAttribute>().FrameworkName;
            Console.Title = $"{CoreSettings.CoinFullName} Node v{version} ({frameworkVersion})";
        }
    }
}