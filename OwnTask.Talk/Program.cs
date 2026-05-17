#region sync

void SyncMethod()
{
    var url = "https://example.com/index.html";

    var content = WebClient.Get(url);
    // Todo: Make this run asynchronous
    Console.WriteLine(content);

    var number = DoSomeCalculation();
    Console.WriteLine($"Number is {number}");
}

#endregion

#region Task

var task = CalculateTheAnswer(
    QuestionOf.Life | QuestionOf.Universe | QuestionOf.Everything
);

#endregion

#region Implementation

int DoSomeCalculation()
{
    return 42;
}

Task<object> CalculateTheAnswer(QuestionOf question)
{
    return Task.FromResult((object)42);
}

[Flags]
public enum QuestionOf
{
    Life,
    Universe,
    Everything
}

public static class WebClient
{
    public static string Get(string url) => url;

    public static void GetAsync(string url, Action<string> onCompleted)
    {
    }

    public static void GetAsync(string url, Action<string> onCompleted, Action<Exception> onError)
    {
    }
}

internal class Test
{
    private void Start()
    {
        for (var i = 0; i < 100; i++)
        {
            Thread.Sleep(500);
            Console.WriteLine(i);
        }
    }
}

#endregion