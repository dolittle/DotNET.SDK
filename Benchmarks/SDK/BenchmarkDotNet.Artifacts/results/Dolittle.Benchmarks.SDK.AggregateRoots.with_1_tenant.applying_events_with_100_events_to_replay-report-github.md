``` ini

BenchmarkDotNet=v0.13.1, OS=macOS Monterey 12.1 (21C52) [Darwin 21.2.0]
Apple M1 Max, 1 CPU, 10 logical and 10 physical cores
.NET SDK=6.0.101
  [Host]     : .NET 6.0.1 (6.0.121.56705), Arm64 RyuJIT
  Job-BRIUJW : .NET 6.0.1 (6.0.121.56705), Arm64 RyuJIT

PowerPlanMode=00000000-0000-0000-0000-000000000000  InvocationCount=1  IterationCount=10  
LaunchCount=1  RunStrategy=ColdStart  UnrollFactor=1  
WarmupCount=0  

```

| Method                                      |        Mean |     Error |    StdDev |      Median |         Min |         Max |      Gen 0 |      Gen 1 | Allocated |
|---------------------------------------------|------------:|----------:|----------:|------------:|------------:|------------:|-----------:|-----------:|----------:|
| Applying1EventWith100EventsToReplay         |    37.74 ms |  1.843 ms |  1.219 ms |    37.69 ms |    36.43 ms |    40.24 ms |          - |          - |    667 KB |
| Applying100EventsWith100EventsToReplay      |    76.45 ms |  5.100 ms |  3.373 ms |    75.33 ms |    71.48 ms |    80.89 ms |          - |          - |  1,491 KB |
| Applying1Event100TimesWith100EventsToReplay | 1,445.49 ms | 93.898 ms | 62.108 ms | 1,441.30 ms | 1,361.21 ms | 1,546.32 ms | 37000.0000 | 11000.0000 | 97,139 KB |
