``` ini

BenchmarkDotNet=v0.13.1, OS=macOS Monterey 12.1 (21C52) [Darwin 21.2.0]
Apple M1 Max, 1 CPU, 10 logical and 10 physical cores
.NET SDK=6.0.101
  [Host]     : .NET 6.0.1 (6.0.121.56705), Arm64 RyuJIT
  Job-WIQYRU : .NET 6.0.1 (6.0.121.56705), Arm64 RyuJIT

PowerPlanMode=00000000-0000-0000-0000-000000000000  InvocationCount=1  IterationCount=10  
LaunchCount=1  RunStrategy=ColdStart  UnrollFactor=1  
WarmupCount=0  

```

| Method                                    |        Mean |     Error |    StdDev |      Median |         Min |         Max |      Gen 0 |      Gen 1 |  Allocated |
|-------------------------------------------|------------:|----------:|----------:|------------:|------------:|------------:|-----------:|-----------:|-----------:|
| Applying1EventWithNothingToReplay         |    59.56 ms |  7.775 ms |  5.142 ms |    58.39 ms |    55.81 ms |    73.30 ms |          - |          - |     179 KB |
| Applying100EventsWithNothingToReplay      |   190.28 ms | 11.171 ms |  7.389 ms |   188.99 ms |   178.77 ms |   203.27 ms |  1000.0000 |          - |   3,453 KB |
| Applying1Event100TimesWithNothingToReplay | 1,348.30 ms | 35.616 ms | 23.558 ms | 1,340.99 ms | 1,319.27 ms | 1,399.02 ms | 56000.0000 | 15000.0000 | 136,785 KB |
