namespace OwnTask.Tests;

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
}