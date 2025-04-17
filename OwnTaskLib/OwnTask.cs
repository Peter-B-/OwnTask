using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace OwnTaskLib;

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

    public static OwnTask Run(Func<OwnTask> action)
    {
        var task = new OwnTask();
        OwnThreadPool.QueueUserWorkItem(() =>
        {
            try
            {
                var nextTask = action();
                nextTask.ContinueWith(() =>
                {
                    if (nextTask._exception is null)
                        task.SetResult();
                    else
                        task.SetException(nextTask._exception);
                });
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