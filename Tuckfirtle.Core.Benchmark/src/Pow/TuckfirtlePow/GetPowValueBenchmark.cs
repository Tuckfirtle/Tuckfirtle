using BenchmarkDotNet.Attributes;

namespace Tuckfirtle.Core.Benchmark.Pow.TuckfirtlePow
{
    [Config(typeof(Config))]
    [RankColumn]
    public class GetPowValueBenchmark
    {
        private Core.Pow.TuckfirtlePow TuckfirtlePow { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            TuckfirtlePow = new Core.Pow.TuckfirtlePow();
        }

        [Benchmark]
        public byte[] GetPowValueTest()
        {
            return TuckfirtlePow.GetPowValue("");
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            TuckfirtlePow?.Dispose();
        }
    }
}