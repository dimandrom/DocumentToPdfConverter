using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Columns;

namespace DocumentToPdfConverter.Benchmarks;

public static class Program
{
    public static void Main(string[] args)
    {
        var config = DefaultConfig.Instance.AddColumn(StatisticColumn.P95);
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
    }
}
