using System;
using System.IO;
using Newtonsoft.Json;

namespace Tuckfirtle.Miner.Config.Json
{
    public class JsonConfig : IConfig, IDisposable
    {
        public int DonateLevel => ConfigModel.DonateLevel;

        public int PrintTime => ConfigModel.PrintTime;

        public bool SafeMode => ConfigModel.SafeMode;

        public MiningMode MiningMode => ConfigModel.MiningMode;

        public MiningThread[] Threads => ConfigModel.Threads;

        private string SettingFilePath { get; }

        private ConfigModel ConfigModel { get; set; } = new ConfigModel();

        public JsonConfig(string settingFilePath)
        {
            SettingFilePath = settingFilePath;
        }

        public void CreateConfig()
        {
            SaveConfig();
        }

        public bool LoadConfig()
        {
            if (!File.Exists(SettingFilePath))
                return false;

            using (var streamReader = new StreamReader(new FileStream(SettingFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                var jsonSerializer = new JsonSerializer();
                ConfigModel = jsonSerializer.Deserialize<ConfigModel>(new JsonTextReader(streamReader));
            }

            ValidateConfig();

            return true;
        }

        private void SaveConfig()
        {
            using (var streamWriter = new StreamWriter(new FileStream(SettingFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)))
            {
                var jsonSerializer = new JsonSerializer { Formatting = Formatting.Indented };
                jsonSerializer.Serialize(streamWriter, ConfigModel);
            }
        }

        private void ValidateConfig()
        {
            var config = ConfigModel;

            if (config.DonateLevel < 1)
                config.DonateLevel = 1;

            if (config.DonateLevel > 100)
                config.DonateLevel = 100;

            if (config.PrintTime < 1)
                config.PrintTime = 1;

            if (config.SafeMode && config.Threads.Length > Environment.ProcessorCount)
                throw new ArgumentException($"Excessive amount of thread allocated which may cause unstable results. Use \"{nameof(ConfigModel.SafeMode)}: false\" if you intend to use this configuration.");

        }

        public void Dispose()
        {
            SaveConfig();
        }
    }
}