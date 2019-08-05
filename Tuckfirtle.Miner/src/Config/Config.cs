// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.IO;
using Tuckfirtle.Miner.Config.Model;

namespace Tuckfirtle.Miner.Config
{
    internal abstract class Config : IConfig, IDisposable
    {
        public int DonateLevel => ConfigModel.DonateLevel;

        public int PrintTime => ConfigModel.PrintTime;

        public bool SafeMode => ConfigModel.SafeMode;

        public MiningMode MiningMode => ConfigModel.MiningMode;

        public MiningThread[] Threads => ConfigModel.Threads;

        public string ConfigFilePath { get; }

        protected ConfigModel ConfigModel { get; set; } = new ConfigModel();

        protected Config(string configFilePath)
        {
            ConfigFilePath = configFilePath;
        }

        public bool IsConfigFileExist()
        {
            return File.Exists(ConfigFilePath);
        }

        public abstract void LoadConfig();

        public abstract void SaveConfig();

        public void Dispose()
        {
            SaveConfig();
        }
    }
}