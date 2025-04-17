using Shouldly;
using TUnit.Assertions.Exceptions;

namespace OwnTaskLib.Tests;

public class Await
{
    [Test]
    public async Task OwnTask_As_Task()
    {
        await OwnTask.Delay(1);
        await OwnTask.Delay(1);
    }

    [Test]
    public async OwnTask AwaitOwnTask_As_OwnTask()
    {
        await OwnTask.Delay(1);
        await OwnTask.Delay(1);
    }

    [Test]
    public async OwnTask AwaitTask_As_OwnTask()
    {
        await Task.Delay(1);
        await Task.Delay(1);
    }

    [Test]
    public async Task ThrowException()
    {
        OwnTask t = new();
        
        t.SetException(TestException.Default);

        try
        {
            await t;
            throw new Exception("Should throw");
        }
        catch (TestException)
        {
            
        }
    }
    
    [Test]
    public async Task ThrowException_Cascading()
    {
        OwnTask t = new();
        
        t.SetException(TestException.Default);

        async OwnTask AwaitT() => await t;

        try
        {
            await AwaitT();
            throw new Exception("Should throw");
        }
        catch (TestException)
        {
            
        }
    }
}