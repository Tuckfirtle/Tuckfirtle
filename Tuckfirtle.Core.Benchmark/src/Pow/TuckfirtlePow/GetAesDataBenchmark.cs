﻿using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace Tuckfirtle.Core.Benchmark.Pow.TuckfirtlePow
{
    [Config(typeof(Config))]
    [RankColumn]
    public class GetAesDataBenchmark
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
        public byte[] GetAesDataTest()
        {
            return Core.Pow.TuckfirtlePow.GetAesData(PowData);
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            TuckfirtlePow?.Dispose();
        }
    }
}
