namespace OwnTaskLib.Tests;

public class Run
{
    [Test]
    public async OwnTask RunAction()
    {
        await OwnTask.Run(() => { });
    }

    [Test]
    public async OwnTask RunFunc()
    {
        await OwnTask.Run(() => OwnTask.Delay(1));
    }
    
    [Test]
    public async OwnTask RunFunc_Nested()
    {
        await OwnTask.Run(
            () => OwnTask.Run(
                () => OwnTask.Delay(1)));
    }
    
    [Test]
    public async Task ThrowException_Direct()
    {
        await OwnTask.Run(() => throw TestException.Default)
            .ShouldThrow<TestException>();
    }
    
    [Test]
    public async Task ThrowException_Async()
    {
        await OwnTask.Run(async () => throw TestException.Default)
            .ShouldThrow<TestException>();
    }
    
    [Test]
    public async Task ThrowException_AsyncAfterAwait()
    {
        await OwnTask.Run(async () =>
            {
                await OwnTask.Delay(1);
                throw TestException.Default;
            })
            .ShouldThrow<TestException>();
    }
    
    [Test]
    public async Task ThrowException_FailedTask()
    {
        OwnTask t = new();
        t.SetException(TestException.Default);
        
        await OwnTask.Run(() => t)
            .ShouldThrow<TestException>();
    }
}