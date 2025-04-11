using Shouldly;

namespace OwnTask.Tests;

[Timeout(100)]
public class Wait
{
    [Test]
    public async Task SetResult_Completes(CancellationToken timeoutToken)
    {
        var t = new OwnTask();

        var waitTask = Task.Run(() => t.Wait());
        waitTask.IsCompleted.ShouldBeFalse();

        t.SetResult();
        await waitTask.WaitAsync(timeoutToken);
    }

    [Test]
    public async Task SetException_Throws(CancellationToken timeoutToken)
    {
        var t = new OwnTask();

        var waitTask = Task.Run(() => t.Wait());
        waitTask.IsCompleted.ShouldBeFalse();

        t.SetException(TestException.Default);
        await waitTask.WaitAsync(timeoutToken)
            .ShouldThrowAsync<TestException>();
    }
}