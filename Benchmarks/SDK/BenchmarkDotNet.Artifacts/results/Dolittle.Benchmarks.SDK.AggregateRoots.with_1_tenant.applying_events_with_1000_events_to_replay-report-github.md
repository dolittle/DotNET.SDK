``` ini

BenchmarkDotNet=v0.13.1, OS=macOS Monterey 12.1 (21C52) [Darwin 21.2.0]
Apple M1 Max, 1 CPU, 10 logical and 10 physical cores
.NET SDK=6.0.101
  [Host]     : .NET 6.0.1 (6.0.121.56705), Arm64 RyuJIT
  Job-LGXKSP : .NET 6.0.1 (6.0.121.56705), Arm64 RyuJIT

PowerPlanMode=00000000-0000-0000-0000-000000000000  InvocationCount=1  IterationCount=10  
LaunchCount=1  RunStrategy=ColdStart  UnrollFactor=1  
WarmupCount=0  

```

| Method                                       |        Mean |      Error |     StdDev |      Median |         Min |         Max |       Gen 0 |      Gen 1 |      Gen 2 | Allocated |
|----------------------------------------------|------------:|-----------:|-----------:|------------:|------------:|------------:|------------:|-----------:|-----------:|----------:|
| Applying1EventWith1000EventsToReplay         |    83.77 ms |   6.595 ms |   4.362 ms |    83.17 ms |    77.31 ms |    92.01 ms |   1000.0000 |          - |          - |      6 MB |
| Applying100EventsWith1000EventsToReplay      |   117.62 ms |   5.829 ms |   3.856 ms |   117.62 ms |   112.28 ms |   122.22 ms |   1000.0000 |          - |          - |      7 MB |
| Applying1Event100TimesWith1000EventsToReplay | 4,163.71 ms | 284.978 ms | 188.496 ms | 4,106.63 ms | 3,962.99 ms | 4,466.38 ms | 219000.0000 | 49000.0000 | 11000.0000 |    647 MB |
