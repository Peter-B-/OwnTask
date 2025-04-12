
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

#endregion