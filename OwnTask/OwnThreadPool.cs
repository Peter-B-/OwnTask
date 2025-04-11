using System.Collections.Concurrent;

namespace OwnTask;

public static class OwnThreadPool
{
    private static readonly BlockingCollection<(Action, ExecutionContext?)> Queue = new();

    static OwnThreadPool()
    {
        for (var i = 0; i < Environment.ProcessorCount; i++)
            new Thread(WorkerLoop)
            {
                IsBackground = true
            }.Start();
    }

    private static void WorkerLoop()
    {
        while (true)
            try
            {
                var (action, executionContext) = Queue.Take();
                if (executionContext is not null)
                    ExecutionContext.Run(executionContext, a => ((Action)a!)(), action);
                else
                    action();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
    }

    public static void QueueUserWorkItem(Action action)
    {
        Queue.Add((action, ExecutionContext.Capture()));
    }
}