// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

namespace Tuckfirtle.Miner.Config.Model
{
    internal sealed class MiningThread
    {
        public int AffinityToCpu { get; set; } = -1;
    }
}