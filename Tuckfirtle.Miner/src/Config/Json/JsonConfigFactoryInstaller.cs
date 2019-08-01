using Microsoft.Extensions.DependencyInjection;
using TheDialgaTeam.Core.DependencyInjection;

namespace Tuckfirtle.Miner.Config.Json
{
    public sealed class JsonConfigFactoryInstaller : FactoryInstaller
    {
        private string SettingFilePath { get; }

        public JsonConfigFactoryInstaller(string settingFilePath)
        {
            SettingFilePath = settingFilePath;
        }

        public override void Install(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IConfig>(new JsonConfig(SettingFilePath));
        }
    }
}