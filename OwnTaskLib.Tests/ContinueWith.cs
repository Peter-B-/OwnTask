using Shouldly;

namespace OwnTaskLib.Tests;

public class ContinueWith
{
    [Test]
    public async Task SetResult_CompletesContinuedTask(CancellationToken timeoutToken)
    {
        var t = new OwnTask();
        var t2 = t.ContinueWith(() => { });

        t.SetResult();

        await t2.ShouldComplete(timeoutToken);
    }

    [Test]
    public async Task SetResult_ContinuationIsExecuted(CancellationToken timeoutToken)
    {
        var executed = false;
        var t = new OwnTask();
        var t2 = t.ContinueWith(() => executed = true);

        t.SetResult();

        await t2.ShouldComplete(timeoutToken);
        executed.ShouldBeTrue();
    }


    [Test]
    public async Task SetException_ContinuationIsExecuted(CancellationToken timeoutToken)
    {
        var executed = false;
        var t = new OwnTask();
        var t2 = t.ContinueWith(() => executed = true);

        t.SetException(TestException.Default);

        await t2.ShouldComplete(timeoutToken);
        executed.ShouldBeTrue();
    }
}