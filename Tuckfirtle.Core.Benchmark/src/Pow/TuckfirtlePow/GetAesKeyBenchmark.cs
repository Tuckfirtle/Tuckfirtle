using BenchmarkDotNet.Attributes;

namespace Tuckfirtle.Core.Benchmark.Pow.TuckfirtlePow
{
    [Config(typeof(Config))]
    [RankColumn]
    public class GetAesKeyBenchmark
    {
        private Core.Pow.TuckfirtlePow TuckfirtlePow { get; set; }

        private byte[] PowData { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            TuckfirtlePow = new Core.Pow.TuckfirtlePow();
            PowData = TuckfirtlePow.GetPowData("");
        }

        [Benchmark]
        public byte[] GetAesKeyTest()
        {
            return TuckfirtlePow.GetAesKey(PowData);
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            TuckfirtlePow?.Dispose();
        }
    }
}