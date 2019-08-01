namespace Tuckfirtle.Miner.Config
{
    public interface IConfig
    {
        int DonateLevel { get; }

        int PrintTime { get; }

        bool SafeMode { get; }

        MiningMode MiningMode { get; }

        MiningThread[] Threads { get; }

        void CreateConfig();

        bool LoadConfig();
    }
}