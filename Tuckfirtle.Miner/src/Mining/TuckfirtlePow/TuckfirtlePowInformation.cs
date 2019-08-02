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