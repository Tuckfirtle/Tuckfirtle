// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Tuckfirtle.Miner.Config.Model;

namespace Tuckfirtle.Miner.Config
{
    internal sealed class JsonConfig : Config
    {
        public JsonConfig(string configFilePath) : base(configFilePath)
        {
        }

        public override void LoadConfig()
        {
            using (var streamReader = new StreamReader(new FileStream(ConfigFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                var jsonSerializer = new JsonSerializer();
                ConfigModel = jsonSerializer.Deserialize<ConfigModel>(new JsonTextReader(streamReader));
            }

            ValidateConfig();
        }

        public override void SaveConfig()
        {
            using (var streamWriter = new StreamWriter(new FileStream(ConfigFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)))
            {
                var jsonSerializer = new JsonSerializer { Formatting = Formatting.Indented };
                jsonSerializer.Serialize(streamWriter, ConfigModel);
            }
        }

        private void ValidateConfig()
        {
            var config = ConfigModel;

            if (config.DonateLevel < 0)
                config.DonateLevel = 0;

            if (config.DonateLevel > 100)
                config.DonateLevel = 100;

            if (config.PrintTime < 1)
                config.PrintTime = 1;

            var miningThreads = config.Threads;

            if (config.SafeMode && miningThreads.Length > Environment.ProcessorCount)
                throw new ArgumentException($"Excessive amount of thread allocated which may cause unstable results. Use \"{nameof(ConfigModel.SafeMode)}: false, if you intend to use this configuration.");

            foreach (var configThread in miningThreads)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    configThread.AffinityToCpu = -1;
                else
                {
                    if (configThread.AffinityToCpu < -1)
                        configThread.AffinityToCpu = -1;

                    if (config.SafeMode && configThread.AffinityToCpu >= Environment.ProcessorCount)
                        throw new ArgumentException($"Invalid affinity set for mining thread. Use \"{nameof(ConfigModel.SafeMode)}\": false, if you intend to use this configuration.");
                }
            }
        }
    }
}