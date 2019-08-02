using Microsoft.Extensions.DependencyInjection;
using TheDialgaTeam.Core.DependencyInjection.Factory;
using TheDialgaTeam.Core.DependencyInjection.Service;

namespace Tuckfirtle.Miner.Bootstrap
{
    internal sealed class BootstrapFactoryInstaller : IFactoryInstaller
    {
        public void Install(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IServiceExecutor, BootstrapServiceExecutor>();
        }
    }
}