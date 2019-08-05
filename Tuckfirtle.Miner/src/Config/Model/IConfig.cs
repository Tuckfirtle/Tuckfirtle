// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

namespace Tuckfirtle.Miner.Config.Model
{
    internal interface IConfig
    {
        int DonateLevel { get; }

        int PrintTime { get; }

        bool SafeMode { get; }

        MiningMode MiningMode { get; }

        MiningThread[] Threads { get; }
    }
}