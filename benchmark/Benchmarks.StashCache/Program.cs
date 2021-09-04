using BenchmarkDotNet.Running;

namespace Benchmarks.StashCache
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<LocalCacheBenchmarks>();
        }
    }
}
