using BenchmarkDotNet.Running;

namespace LoggingBenchmark;

internal static class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<LoggingBenchmark>();
    }
}