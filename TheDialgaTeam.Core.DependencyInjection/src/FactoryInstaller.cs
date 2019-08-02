using Microsoft.Extensions.DependencyInjection;

namespace TheDialgaTeam.Core.DependencyInjection
{
    public abstract class FactoryInstaller
    {
        public abstract void Install(IServiceCollection serviceCollection);
    }
}