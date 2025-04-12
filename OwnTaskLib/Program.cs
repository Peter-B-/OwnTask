using OwnTaskLib;

await MultiStepAsync();

async OwnTask MultiStepAsync()
{
    await OwnTask.Delay(200);
    Console.WriteLine("Hallo");
    await OwnTask.Delay(200);
    Console.WriteLine("1");
    await OwnTask.Delay(200);
    //throw new Exception("Test");
    Console.WriteLine("2");
    await OwnTask.Delay(200);
    Console.WriteLine("3");
    await OwnTask.Delay(200);
}


try
{
    OwnTask.Iterate(MultiStep()).Wait();
}
catch (Exception e)
{
    Console.WriteLine(e);
}

IEnumerable<OwnTask> MultiStep()
{
    yield return OwnTask.Delay(200);
    Console.WriteLine("Hallo");
    yield return OwnTask.Delay(200);
    Console.WriteLine("1");
    yield return OwnTask.Delay(200);
    //throw new Exception("Test");
    Console.WriteLine("2");
    yield return OwnTask.Delay(200);
    Console.WriteLine("3");
    yield return OwnTask.Delay(200);
}


Console.WriteLine("Hello");
OwnTask.Delay(500)
    .ContinueWith(() =>
    {
        Console.WriteLine("friends");
        return OwnTask.Delay(500);
    })
    .ContinueWith(() => Console.WriteLine("and"))
    .ContinueWith(() => OwnTask.Delay(500))
    .ContinueWith(() => Console.WriteLine("foes"))
    .Wait();


List<OwnTask> tasks = new();
AsyncLocal<int> asyncLocal = new();
for (var i = 0; i < 100; i++)
{
    asyncLocal.Value = i;
    var task =
        OwnTask.Delay(500)
            .ContinueWith(() => Console.WriteLine(asyncLocal.Value))
            .ContinueWith(() => Console.WriteLine("completed"));

    tasks.Add(task);
}

OwnTask.WhenAll(tasks).Wait();