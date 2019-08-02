using Microsoft.Extensions.DependencyInjection;
using TheDialgaTeam.Core.DependencyInjection;
using TheDialgaTeam.Core.DependencyInjection.Factory;
using Tuckfirtle.Miner.Bootstrap.Service;

namespace Tuckfirtle.Miner.Bootstrap.Factory
{
    internal sealed class MiningBootstrapFactoryInstaller : IFactoryInstaller
    {
        public void Install(IServiceCollection serviceCollection)
        {
            serviceCollection.AddInterfacesAsSingleton<MiningBootstrapService>();
        }
    }
}