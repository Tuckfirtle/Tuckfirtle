namespace Tuckfirtle.Miner.Config.Model
{
    internal sealed class ConfigModel : IConfig
    {
        public int DonateLevel { get; set; }

        public int PrintTime { get; set; } = 10;

        public bool SafeMode { get; set; } = true;

        public MiningMode MiningMode { get; set; } = MiningMode.Test;

        public MiningThread[] Threads { get; set; } = { new MiningThread() };
    }
}