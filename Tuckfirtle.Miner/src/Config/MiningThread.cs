using System.Threading;

namespace Tuckfirtle.Miner.Config
{
    public sealed class MiningThread
    {
        public ThreadPriority ThreadPriority { get; set; } = ThreadPriority.Normal;
    }
}