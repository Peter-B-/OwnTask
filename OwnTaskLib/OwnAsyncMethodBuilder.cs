using System.Runtime.CompilerServices;

namespace OwnTaskLib;

public readonly struct OwnAsyncMethodBuilder
{
    private OwnAsyncMethodBuilder(OwnTask task)
    {
        Task = task;
    }

    public static OwnAsyncMethodBuilder Create()
    {
        return new OwnAsyncMethodBuilder(new OwnTask());
    }

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