// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using TheDialgaTeam.Core.DependencyInjection.Service;
using TheDialgaTeam.Core.Logger;
using Tuckfirtle.Core.Pow;
using Tuckfirtle.Core.Utility;
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
            var consoleLogger = ConsoleLogger;
            var config = Config;
            var threadCount = config.Threads.Length;

            var benchmarkTest = new TuckfirtlePowBenchmarkTest();
            var miners = new TuckfirtlePowMiner[threadCount];

            consoleLogger.LogMessage("use benchmark");

            benchmarkTest.NewJob += (test, information) =>
            {
                consoleLogger.LogMessage(new ConsoleMessageBuilder()
                    .Write("new job ", ConsoleColor.Magenta)
                    .WriteLine($"diff {DifficultyUtility.GetDifficulty(information.TargetPowValue)} algo {nameof(TuckfirtlePow)} height {information.Height}", false)
                    .Build());
            };

            benchmarkTest.ShareResult += (test, accepted, reason) =>
            {
                if (accepted)
                {
                    consoleLogger.LogMessage(new ConsoleMessageBuilder()
                        .Write("accecpted ", ConsoleColor.Green)
                        .WriteLine($"({test.TotalAcceptedShare}/{test.TotalRejectedShare})", false)
                        .Build());
                }
                else
                {
                    consoleLogger.LogMessage(new ConsoleMessageBuilder()
                        .Write("rejected ", ConsoleColor.Red)
                        .Write($"({test.TotalAcceptedShare}/{test.TotalRejectedShare}) ", false)
                        .WriteLine(reason, false)
                        .Build());
                }
            };

            benchmarkTest.StartTest();

            for (var i = 0; i < threadCount; i++)
            {
                miners[i] = new TuckfirtlePowMiner(new TuckfirtlePowMinerInformation
                {
                    ThreadAffinity = config.Threads[i].AffinityToCpu < 0 ? 0 : 1 << config.Threads[i].AffinityToCpu,
                    StartingNonce = ulong.MaxValue / (ulong) threadCount * (ulong) i
                }, benchmarkTest.PowInformation, benchmarkTest.SubmitTestResult);

                miners[i].StartMining();
            }
        }
    }
}