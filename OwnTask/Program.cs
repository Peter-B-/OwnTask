await MultiStepAsync();

async OwnTask.OwnTask MultiStepAsync()
{
    await OwnTask.OwnTask.Delay(200);
    Console.WriteLine("Hallo");
    await OwnTask.OwnTask.Delay(200);
    Console.WriteLine("1");
    await OwnTask.OwnTask.Delay(200);
    //throw new Exception("Test");
    Console.WriteLine("2");
    await OwnTask.OwnTask.Delay(200);
    Console.WriteLine("3");
    await OwnTask.OwnTask.Delay(200);
}


try
{
    OwnTask.OwnTask.Iterate(MultiStep()).Wait();
}
catch (Exception e)
{
    Console.WriteLine(e);
}

IEnumerable<OwnTask.OwnTask> MultiStep()
{
    yield return OwnTask.OwnTask.Delay(200);
    Console.WriteLine("Hallo");
    yield return OwnTask.OwnTask.Delay(200);
    Console.WriteLine("1");
    yield return OwnTask.OwnTask.Delay(200);
    //throw new Exception("Test");
    Console.WriteLine("2");
    yield return OwnTask.OwnTask.Delay(200);
    Console.WriteLine("3");
    yield return OwnTask.OwnTask.Delay(200);
}


Console.WriteLine("Hello");
OwnTask.OwnTask.Delay(500)
    .ContinueWith(() =>
    {
        Console.WriteLine("friends");
        return OwnTask.OwnTask.Delay(500);
    })
    .ContinueWith(() => Console.WriteLine("and"))
    .ContinueWith(() => OwnTask.OwnTask.Delay(500))
    .ContinueWith(() => Console.WriteLine("foes"))
    .Wait();


List<OwnTask.OwnTask> tasks = new();
AsyncLocal<int> asyncLocal = new();
for (var i = 0; i < 100; i++)
{
    asyncLocal.Value = i;
    var task =
        OwnTask.OwnTask.Delay(500)
            .ContinueWith(() => Console.WriteLine(asyncLocal.Value))
            .ContinueWith(() => Console.WriteLine("completed"));

    tasks.Add(task);
}

OwnTask.OwnTask.WhenAll(tasks).Wait();