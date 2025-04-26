using BenchmarkDotNet.Attributes;

namespace OwnTaskLib.Benchmarks;

[MemoryDiagnoser(false)]
[HideColumns("StdDev", "RatioSD")]
public class AwaitEmptyContinueWith
{
    [Benchmark]
    public async OwnTask MyTask()
    {
        await OwnTask.Run(() => { })
            .ContinueWith(() => { });
    }

    [Benchmark(Baseline = true)]
    public async OwnTask ThierTask()
    {
        await Task.Run(() => { })
            .ContinueWith(_ => { });
    }
}