namespace Tuckfirtle.Miner.Config.Model
{
    internal interface IConfig
    {
        int DonateLevel { get; }

        int PrintTime { get; }

        bool SafeMode { get; }

        MiningMode MiningMode { get; }

        MiningThread[] Threads { get; }
    }
}