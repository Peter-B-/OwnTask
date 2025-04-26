
Task<object> task = CalculateTheAnswer(
    QuestionOf.Life | QuestionOf.Universe | QuestionOf.Everything
    );



#region Implementation

async Task<object> CalculateTheAnswer(QuestionOf question) => 42;

[Flags]
public enum QuestionOf
{
    Life,
    Universe,
    Everything
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