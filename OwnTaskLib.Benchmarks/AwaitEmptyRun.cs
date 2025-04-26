using BenchmarkDotNet.Attributes;

namespace OwnTaskLib.Benchmarks;

[MemoryDiagnoser(false)]
[HideColumns("StdDev", "RatioSD")]
public class AwaitEmptyRun
{
    [Benchmark]
    public async OwnTask MyTask()
    {
        await OwnTask.Run(() => { });
    }

    [Benchmark(Baseline = true)]
    public async OwnTask ThierTask()
    {
        await Task.Run(() => { });
    }
}