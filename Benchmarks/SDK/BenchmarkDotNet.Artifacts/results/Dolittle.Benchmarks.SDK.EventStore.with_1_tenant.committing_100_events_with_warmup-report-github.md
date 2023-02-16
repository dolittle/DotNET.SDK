``` ini

BenchmarkDotNet=v0.13.1, OS=macOS Monterey 12.1 (21C52) [Darwin 21.2.0]
Apple M1 Max, 1 CPU, 10 logical and 10 physical cores
.NET SDK=6.0.101
  [Host]     : .NET 6.0.1 (6.0.121.56705), Arm64 RyuJIT
  Job-GYTQPF : .NET 6.0.1 (6.0.121.56705), Arm64 RyuJIT

PowerPlanMode=00000000-0000-0000-0000-000000000000  InvocationCount=1  IterationCount=10  
LaunchCount=1  RunStrategy=ColdStart  UnrollFactor=1  
WarmupCount=0  

```

| Method        |        Mean |     Error |    StdDev |      Median |         Min |         Max | Allocated |
|---------------|------------:|----------:|----------:|------------:|------------:|------------:|----------:|
| Commit100Loop | 3,272.15 μs | 355.58 μs | 235.19 μs | 3,325.33 μs | 2,890.56 μs | 3,659.82 μs |  18,791 B |
| Commit100     |   472.43 μs |  83.70 μs |  55.36 μs |   466.25 μs |   416.01 μs |   564.82 μs |   9,146 B |
| Commit1       |    46.69 μs |  16.67 μs |  11.03 μs |    43.40 μs |    35.70 μs |    70.44 μs |     441 B |
