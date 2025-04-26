using BenchmarkDotNet.Attributes;

namespace OwnTaskLib.Benchmarks;

[MemoryDiagnoser(false)]
[HideColumns("StdDev", "RatioSD")]
public class ParallelAwaitEmptyContinueWith
{
    [Params(10, 100, 1000)]
    public int NoOfTasks { get; set; }
    
    [Benchmark]
    public async OwnTask MyTask()
    {
        var tasks = new List<OwnTask>(NoOfTasks);
        for (int i = 0; i < NoOfTasks; i++)
            tasks.Add(
                OwnTask.Run(() => { })
                    .ContinueWith(() => { })
            );
        
        foreach (var task in tasks)
            await task;
    }

    [Benchmark(Baseline = true)]
    public async OwnTask ThierTask()
    {
        var tasks = new List<Task>(NoOfTasks);
        for (int i = 0; i < NoOfTasks; i++)
            tasks.Add(
                Task.Run(() => { })
                    .ContinueWith(_ => { })
            );
        
        foreach (var task in tasks)
            await task;
    }
}