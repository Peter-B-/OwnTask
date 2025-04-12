using BenchmarkDotNet.Attributes;

namespace OwnTask.Benchmarks;

public class AwaitEmptyContinueWith
{
    [Benchmark]
    public async OwnTaskLib.OwnTask MyTask()
    {
        await OwnTaskLib.OwnTask.Run(() => { })
            .ContinueWith(() => { });
    }

    [Benchmark(Baseline = true)]
    public async OwnTaskLib.OwnTask ThierTask()
    {
        await Task.Run(() => { })
            .ContinueWith(_ => { });
    }
}