



void SyncMethod()
{
    var url = "https://example.com/index.html";
    var content = WebClient.Get(url);
    Console.WriteLine(content);
    
    var number = DoSomeCalculation();
    Console.WriteLine($"Number is {number}");
}

#region async

Task<object> task = CalculateTheAnswer(
    QuestionOf.Life | QuestionOf.Universe | QuestionOf.Everything
);

#endregion

#region Implementation

int DoSomeCalculation() => 42;

Task<object> CalculateTheAnswer(QuestionOf question) => Task.FromResult((object)42);

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

class Test
{
    void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            Thread.Sleep(500);
            Console.WriteLine(i);
        }
    }
}

#endregion