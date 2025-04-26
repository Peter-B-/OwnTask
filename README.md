# OwnTask

## Introduction
The implementation shown here, is greatly inspired by the Deep .Net episode
[Writing async/await from scratch in C#](https://www.youtube.com/watch?v=R-z2Hv-7nxk)
with Stephen Toub and Scott Hanselman.

This project demonstrates how Task and async/await work under the hood in C#. 
By implementing our own versions of these features, we gain a deeper understanding 
of the asynchronous programming model in .NET.

## Project Structure

The solution consists of the following projects:

- **OwnTaskLib**: Core library implementing custom Task and async/await functionality
  - `OwnTask.cs`: Custom implementation of Task
  - `OwnThreadPool.cs`: Simple thread pool implementation
  - `OwnAsyncMethodBuilder.cs`: Custom implementation of async method builder

- **OwnTaskLib.Tests**: Test suite for the OwnTaskLib functionalities
  - Tests for various async scenarios (Run, Wait, Await, ContinueWith, etc.)
  - Validation of async behavior and exception handling

- **OwnTaskLib.Benchmarks**: Performance benchmarks
  - Comparative benchmarks between standard Task and OwnTask implementations
  - Various async operation performance measurements

- **OwnTask.Talk**: The starting point for my presentation

## Getting Started

This whole repository is about `OwnTask`, so take a look at it to see the implementation.

A good way to learn about the behavior of `OwnTask` is to modify and extend the unit tests.
They use the great [TUnit](https://github.com/thomhurst/TUnit) from Tom Longhurst.  
Please check out [tunit.dev](https://tunit.dev/) for setup instructions.

This repository also includes some simple bechmarks. You can execute them with
```powershell
cd .\OwnTaskLib.Benchmarks\
dotnet run -c Release
```
