using Shouldly;

namespace OwnTaskLib.Tests;

public class AsyncLocal
{
    [Test]
    public async OwnTask InRun()
    {
        AsyncLocal<int> local = new();
        local.Value = 42;

        var value = 0;
        await OwnTask.Run(() => value = local.Value);
        value.ShouldBe(42);
    }

    [Test]
    public async OwnTask InContinueWith()
    {
        AsyncLocal<int> local = new();
        local.Value = 42;

        var value = 0;
        await OwnTask.Run(() => local.Value++)
            .ContinueWith(() => value = local.Value);

        value.ShouldBe(42);
    }
}