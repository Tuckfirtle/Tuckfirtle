using System;
using System.Threading;
using TheDialgaTeam.Core.DependencyInjection.Service;
using TheDialgaTeam.Core.Logger;

namespace Tuckfirtle.Miner.Config
{
    internal sealed class JsonConfigServiceExecutor : IServiceExecutor
    {
        private JsonConfig JsonConfig { get; }

        private IConsoleLogger ConsoleLogger { get; }

        private CancellationTokenSource CancellationTokenSource { get; }

        public JsonConfigServiceExecutor(JsonConfig jsonConfig, IConsoleLogger consoleLogger, CancellationTokenSource cancellationTokenSource)
        {
            JsonConfig = jsonConfig;
            ConsoleLogger = consoleLogger;
            CancellationTokenSource = cancellationTokenSource;
        }

        public void Execute()
        {
            var jsonConfig = JsonConfig;

            if (!jsonConfig.IsConfigFileExist())
            {
                jsonConfig.SaveConfig();

                ConsoleLogger.LogMessage(new ConsoleMessageBuilder()
                    .WriteLine($"Generated configuration file is at: \"{jsonConfig.ConfigFilePath}\"", false)
                    .WriteLine("Press Enter/Return to exit...", false)
                    .Build());

                Console.ReadLine();
                CancellationTokenSource.Cancel();
            }
            else
                jsonConfig.LoadConfig();
        }
    }
}