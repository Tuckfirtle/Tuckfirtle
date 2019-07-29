using BenchmarkDotNet.Running;
using Tuckfirtle.Core.Benchmark.Pow.TuckfirtlePow;

namespace Tuckfirtle.Core.Benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<GetPowValueBenchmark>();
        }
    }
}