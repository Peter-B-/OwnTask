using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

await MultiStepAsync();

async OwnTask MultiStepAsync()
{
    await OwnTask.Delay(200);
    Console.WriteLine("Hallo");
    await OwnTask.Delay(200);
    Console.WriteLine("1");
    await OwnTask.Delay(200);
    //throw new Exception("Test");
    Console.WriteLine("2");
    await OwnTask.Delay(200);
    Console.WriteLine("3");
    await OwnTask.Delay(200);
}


try
{
    OwnTask.Iterate(MultiStep()).Wait();
}
catch (Exception e)
{
    Console.WriteLine(e);
}
IEnumerable<OwnTask> MultiStep()
{
    yield return OwnTask.Delay(200);
    Console.WriteLine("Hallo");
    yield return OwnTask.Delay(200);
    Console.WriteLine("1");
    yield return OwnTask.Delay(200);
    //throw new Exception("Test");
    Console.WriteLine("2");
    yield return OwnTask.Delay(200);
    Console.WriteLine("3");
    yield return OwnTask.Delay(200);
}


Console.WriteLine("Hello");
OwnTask.Delay(500)
    .ContinueWith(() =>
    {
        Console.WriteLine("friends");
        return OwnTask.Delay(500);

    })
    .ContinueWith(() => Console.WriteLine("and"))
    .ContinueWith(() => OwnTask.Delay(500))
    .ContinueWith(() => Console.WriteLine("foes"))
    .Wait();


List<OwnTask> tasks = new();
AsyncLocal<int> asyncLocal = new();
for (var i = 0; i < 100; i++)
{
    asyncLocal.Value = i;
    var task = 
            OwnTask.Delay(500)
                .ContinueWith(() => Console.WriteLine(asyncLocal.Value))
                .ContinueWith(() => Console.WriteLine("completed"));

    tasks.Add(task);
}

OwnTask.WhenAll(tasks).Wait();


[AsyncMethodBuilder(typeof(OwnAsyncMethodBuilder))]
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

            if (_continuation is not null)
                OwnThreadPool.QueueUserWorkItem(() =>
                {
                    if (_context is null)
                        _continuation();
                    else
                        ExecutionContext.Run(_context, a => ((Action)a!)(), _continuation);
                });
        }
    }

    public OwnTask ContinueWith(Action continuation)
    {
        var task = new OwnTask();

        lock (_lock)
        {
            var action = () =>
            {
                try
                {
                    continuation();
                    task.SetResult();
                }
                catch (Exception e)
                {
                    task.SetException(e);
                }
            };

            if (_completed)
            {
                OwnThreadPool.QueueUserWorkItem(action);
            }
            else
            {
                _continuation = action;
                _context = ExecutionContext.Capture();
            }
        }

        return task;
    }


    public OwnTask ContinueWith(Func<OwnTask> continuation)
    {
        var task = new OwnTask();

        lock (_lock)
        {
            var action = () =>
            {
                try
                {
                    var next = continuation();
                    next.ContinueWith(() =>
                    {
                        if (next._exception is null)
                            task.SetResult();
                        else
                            task.SetException(next._exception);
                    });
                }
                catch (Exception e)
                {
                    task.SetException(e);
                }
            };

            if (_completed)
            {
                OwnThreadPool.QueueUserWorkItem(action);
            }
            else
            {
                _continuation = action;
                _context = ExecutionContext.Capture();
            }
        }

        return task;
    }

    public OwnAwaiter GetAwaiter()
    {
        return new OwnAwaiter(this);
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

    public static OwnTask Delay(int time)
    {
        var task = new OwnTask();

        new Timer(_ => task.SetResult(), null, time, Timeout.Infinite);

        return task;
    }

    public static OwnTask WhenAll(IReadOnlyList<OwnTask> tasks)
    {
        OwnTask t = new();

        if (tasks.Count == 0)
        {
            t.SetResult();
        }
        else
        {
            var remaining = tasks.Count;
            var continuation = () =>
            {
                if (Interlocked.Decrement(ref remaining) == 0)
                    t.SetResult();
            };

            foreach (var task in tasks) task.ContinueWith(continuation);
        }

        return t;
    }

    public static OwnTask Iterate(IEnumerable<OwnTask> tasks)
    {
        OwnTask t = new();

        var enumerator = tasks.GetEnumerator();

        void MoveNext()
        {
            try
            {
                if (!enumerator.MoveNext())
                {
                    t.SetResult();
                    return;
                }

                var nextTask = enumerator.Current;
                nextTask.ContinueWith(MoveNext);
            }
            catch (Exception e)
            {
                t.SetException(e);
            }
        }

        MoveNext();

        return t;
    }

    public readonly struct OwnAwaiter(OwnTask task) : INotifyCompletion
    {
        public bool IsCompleted => task.IsCompleted;

        public void OnCompleted(Action continuation)
        {
            task.ContinueWith(continuation);
        }

        public void GetResult()
        {
            task.Wait();
        }
    }
}

public readonly struct OwnAsyncMethodBuilder
{
    private OwnAsyncMethodBuilder(OwnTask task)
    {
        Task = task;
    }

    public static OwnAsyncMethodBuilder Create() => new(new OwnTask());

    public void Start<TStateMachine>(ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine
    {
        stateMachine.MoveNext();
    }

    public void SetStateMachine(IAsyncStateMachine stateMachine)
    {
        // Optional: Implement for debugger support
    }

    public void SetResult()
    {
        Task.SetResult();
    }

    public void SetException(Exception exception)
    {
        Task.SetException(exception);
    }

    public OwnTask Task { get; }

    public void AwaitOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        awaiter.OnCompleted(stateMachine.MoveNext);
    }

    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        awaiter.UnsafeOnCompleted(stateMachine.MoveNext);
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