// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using Microsoft.Extensions.DependencyInjection;
using TheDialgaTeam.Core.DependencyInjection;
using TheDialgaTeam.Core.DependencyInjection.Factory;
using Tuckfirtle.Miner.Bootstrap.Service;
using Tuckfirtle.Miner.Config;

namespace Tuckfirtle.Miner.Bootstrap.Factory
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
            serviceCollection.AddInterfacesAndSelfAsSingleton(new JsonConfig(ConfigFilePath));
            serviceCollection.AddInterfacesAsSingleton<JsonConfigService>();
        }
    }
}