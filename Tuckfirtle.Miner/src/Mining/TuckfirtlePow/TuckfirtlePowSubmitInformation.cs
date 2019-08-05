// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System.Numerics;

namespace Tuckfirtle.Miner.Mining.TuckfirtlePow
{
    internal struct TuckfirtlePowSubmitInformation
    {
        public ulong Height { get; set; }

        public string BlockHeaderTemplate { get; set; }

        public BigInteger PowValue { get; set; }

        public ulong Nonce { get; set; }
    }
}