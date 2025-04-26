using BenchmarkDotNet.Running;
using OwnTaskLib.Benchmarks;

BenchmarkRunner.Run(typeof(AwaitEmptyRun).Assembly);