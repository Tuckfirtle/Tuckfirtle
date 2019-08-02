using System.Threading;

namespace Tuckfirtle.Miner.Config.Model
{
    internal sealed class MiningThread
    {
        public ThreadPriority ThreadPriority { get; set; } = ThreadPriority.Normal;
    }
}