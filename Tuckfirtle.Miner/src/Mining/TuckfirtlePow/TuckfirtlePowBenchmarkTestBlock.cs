// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Numerics;

namespace Tuckfirtle.Miner.Mining.TuckfirtlePow
{
    public class TuckfirtlePowBenchmarkTestBlock
    {
        public byte Version { get; set; }

        public ulong Height { get; set; }

        public DateTimeOffset DateTime { get; set; }

        public BigInteger Nonce { get; set; }

        public BigInteger TargetPowValue { get; set; }
    }
}