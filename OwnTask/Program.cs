using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;

List<OwnTask> tasks = new();
AsyncLocal<int> asyncLocal = new();
for (var i = 0; i < 100; i++)
{
    asyncLocal.Value = i;
    var task = OwnTask.Run(
        () =>
        {
            Thread.Sleep(500);
            Console.WriteLine(asyncLocal.Value);
        });

    tasks.Add(task);
}

foreach (var task in tasks) task.Wait();


public class OwnTask
{
    private readonly Lock _lock = new();
    private bool _completed;
    private ExecutionContext? _context;
    private Action? _continuation;
    private Exception? _exception;

    public bool IsCompleted
    {
        get
        {
            lock (_lock)
            {
                return _completed;
            }
        }
    }

    public void SetResult()
    {
        Complete(null);
    }

    public void SetException(Exception exception)
    {
        Complete(exception);
    }

    private void Complete(Exception? exception)
    {
        lock (_lock)
        {
            if (_completed)
                throw new InvalidOperationException("OwnTask already completed");

            _completed = true;
            _exception = exception;

            if (_continuation != null)
            {
                if (_context is null)
                    OwnThreadPool.QueueUserWorkItem(_continuation);
                else
                    ExecutionContext.Run(_context, a => ((Action)a!)(), _continuation);
            }
        }
    }

    public void ContinueWith(Action continuation)
    {
        lock (_lock)
        {
            if (_completed)
            {
                OwnThreadPool.QueueUserWorkItem(continuation);
            }
            else
            {
                _continuation = continuation;
                _context = ExecutionContext.Capture();
            }
        }
    }

    public static OwnTask Run(Action action)
    {
        var task = new OwnTask();
        OwnThreadPool.QueueUserWorkItem(() =>
        {
            try
            {
                action();
                task.SetResult();
            }
            catch (Exception e)
            {
                task.SetException(e);
            }
        });
        return task;
    }

    public void Wait()
    {
        ManualResetEventSlim? waitHandle = null;

        lock (_lock)
        {
            if (!_completed)
            {
                waitHandle = new ManualResetEventSlim(false);
                ContinueWith(waitHandle.Set);
            }
        }

        waitHandle?.Wait();

        if (_exception is not null)
            ExceptionDispatchInfo.Throw(_exception);
    }
}

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