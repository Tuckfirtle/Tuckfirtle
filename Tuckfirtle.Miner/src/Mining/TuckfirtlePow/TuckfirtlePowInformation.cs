// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System.Numerics;

namespace Tuckfirtle.Miner.Mining.TuckfirtlePow
{
    internal class TuckfirtlePowInformation
    {
        public ulong Height { get; set; }

        public BigInteger TargetPowValue { get; set; }

        public string BlockHeaderTemplate { get; set; }
    }
}