// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

namespace Tuckfirtle.Miner.Mining.TuckfirtlePow
{
    internal struct TuckfirtlePowMinerInformation
    {
        public int ThreadAffinity { get; set; }

        public ulong StartingNonce { get; set; }
    }
}