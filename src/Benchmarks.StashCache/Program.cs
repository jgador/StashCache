using BenchmarkDotNet.Running;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StashCache;
using System;
using System.Threading.Tasks;

namespace Benchmarks.StashCache
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddLogging(configure => configure.AddConsole());
            services.AddStashCache();

            var serviceProvider = services.BuildServiceProvider();

            BenchmarkRunner.Run<LocalCacheBenchmarks>();
        }
    }
}
