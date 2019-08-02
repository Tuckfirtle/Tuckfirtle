using System;
using System.Collections.Generic;
using TheDialgaTeam.Core.DependencyInjection.Service;
using TheDialgaTeam.Core.Logger;
using Tuckfirtle.Miner.Config.Model;
using Tuckfirtle.Miner.Mining.TuckfirtlePow;

namespace Tuckfirtle.Miner.Bootstrap.Service
{
    internal sealed class MiningBootstrapService : IServiceExecutor
    {
        private IConsoleLogger ConsoleLogger { get; }

        private IConfig Config { get; }

        public MiningBootstrapService(IConsoleLogger consoleLogger, IConfig config)
        {
            ConsoleLogger = consoleLogger;
            Config = config;
        }

        public void Execute()
        {
            var config = Config;

            switch (config.MiningMode)
            {
                case MiningMode.Test:
                    ExecuteTestMode();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ExecuteTestMode()
        {
            var config = Config;
            var miners = new TuckfirtlePowMiner[config.Threads.Length];
        }
    }
}