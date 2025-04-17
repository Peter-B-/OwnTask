using TUnit.Assertions.Exceptions;

namespace OwnTaskLib.Tests;

public static class OwnTaskExtensions
{
    public static Task ToTask(this OwnTask task)
    {
        var tcs = new TaskCompletionSource();
        task.ContinueWith(() => { tcs.SetResult(); });
        return tcs.Task;
    }

    public static Task ShouldComplete(this OwnTask ownTask, CancellationToken timeoutToken)
    {
        return ownTask.ToTask().WaitAsync(timeoutToken);
    }
    
    public static async OwnTask ShouldThrow<TException>(this OwnTask task) where TException : Exception
    {
        try
        {
            await task;
        }
        catch (TException e)
        {
            return;
        }
        catch (Exception e)
        {
            throw new AssertionException($"Should throw {typeof(TException)} but threw {e.GetType()}", e);
        }
        throw new AssertionException($"Should throw {typeof(TException)} but did not");
    }

}