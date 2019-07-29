using System.Runtime.InteropServices;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CsProj;

namespace Tuckfirtle.Core.Benchmark
{
    public class Config : ManualConfig
    {
        public Config()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return;

            Add(Job.Clr.With(CsProjClassicNetToolchain.Net472));
            Add(Job.Core.With(CsProjCoreToolchain.NetCoreApp30));
        }
    }
}