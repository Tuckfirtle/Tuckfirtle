using Microsoft.Extensions.DependencyInjection;
using TheDialgaTeam.Core.DependencyInjection.Factory;
using TheDialgaTeam.Core.DependencyInjection.Service;
using Tuckfirtle.Miner.Config.Model;

namespace Tuckfirtle.Miner.Config
{
    internal sealed class JsonConfigFactoryInstaller : IFactoryInstaller
    {
        private string ConfigFilePath { get; }

        public JsonConfigFactoryInstaller(string configFilePath)
        {
            ConfigFilePath = configFilePath;
        }

        public void Install(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(new JsonConfig(ConfigFilePath));
            serviceCollection.AddSingleton<IConfig>(provider => provider.GetRequiredService<JsonConfig>());
            serviceCollection.AddSingleton<IServiceExecutor, JsonConfigServiceExecutor>();
        }
    }
}