namespace OwnTask.Tests;

public class TestException(string message, Exception? innerException = null):Exception(message, innerException)
{
    public static TestException Default => new("Test");
}