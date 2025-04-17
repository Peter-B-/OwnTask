namespace OwnTaskLib.Tests;

public class TUnitBehavior
{
    // Warning: A failed OwnTask will not make a test fail.
    // Return Task for async tests. 
    [Test]
    public OwnTask OwnTaskExceptions_DoNotFail()
    {
        OwnTask t = new();
        
        t.SetException(TestException.Default);
        
        return t;
    }
}