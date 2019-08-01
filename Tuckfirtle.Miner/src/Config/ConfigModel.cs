namespace Tuckfirtle.Miner.Config
{
    public sealed class ConfigModel
    {
        public int DonateLevel { get; set; }

        public int PrintTime { get; set; } = 10;

        public bool SafeMode { get; set; } = true;

        public MiningMode MiningMode { get; set; } = MiningMode.Test;

        public MiningThread[] Threads { get; set; } = { new MiningThread() };
    }
}