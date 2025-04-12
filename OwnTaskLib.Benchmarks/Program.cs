using BenchmarkDotNet.Running;
using OwnTask.Benchmarks;

BenchmarkRunner.Run([
    typeof(AwaitEmptyRun),
    typeof(AwaitEmptyContinueWith)
]);