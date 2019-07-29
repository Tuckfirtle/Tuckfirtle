using BenchmarkDotNet.Attributes;

namespace Tuckfirtle.Core.Benchmark.Pow.TuckfirtlePow
{
    [Config(typeof(Config))]
    [RankColumn]
    public class GetPowDataBenchmark
    {
        private Core.Pow.TuckfirtlePow TuckfirtlePow { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            TuckfirtlePow = new Core.Pow.TuckfirtlePow();
        }

        [Benchmark]
        public byte[] GetPowDataTest()
        {
            return TuckfirtlePow.GetPowData("");
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            TuckfirtlePow?.Dispose();
        }
    }
}