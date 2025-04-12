using BenchmarkDotNet.Attributes;

namespace OwnTask.Benchmarks;

public class AwaitEmptyRun
{
    [Benchmark]
    public async OwnTaskLib.OwnTask MyTask()
    {
        await OwnTaskLib.OwnTask.Run(() => { });
    }

    [Benchmark(Baseline = true)]
    public async OwnTaskLib.OwnTask ThierTask()
    {
        await Task.Run(() => { });
    }
}