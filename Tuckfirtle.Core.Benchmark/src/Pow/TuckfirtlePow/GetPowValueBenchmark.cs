using BenchmarkDotNet.Attributes;

namespace Tuckfirtle.Core.Benchmark.Pow.TuckfirtlePow
{
    [Config(typeof(Config))]
    [RankColumn]
    public class GetPowValueBenchmark
    {
        [Benchmark]
        public byte[] GetPowValueTest()
        {
            return Core.Pow.TuckfirtlePow.GetPowValue("");
        }
    }
}