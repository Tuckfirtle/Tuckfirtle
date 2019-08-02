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