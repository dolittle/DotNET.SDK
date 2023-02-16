``` ini

BenchmarkDotNet=v0.13.1, OS=macOS Monterey 12.1 (21C52) [Darwin 21.2.0]
Apple M1 Max, 1 CPU, 10 logical and 10 physical cores
.NET SDK=6.0.101
  [Host]     : .NET 6.0.1 (6.0.121.56705), Arm64 RyuJIT
  Job-RIUTQV : .NET 6.0.1 (6.0.121.56705), Arm64 RyuJIT

PowerPlanMode=00000000-0000-0000-0000-000000000000  InvocationCount=1  IterationCount=10  
LaunchCount=1  RunStrategy=ColdStart  UnrollFactor=1  
WarmupCount=0  

```

| Method                                    |        Mean |     Error |   StdDev |      Median |         Min |        Max |       Gen 0 |      Gen 1 |  Allocated |
|-------------------------------------------|------------:|----------:|---------:|------------:|------------:|-----------:|------------:|-----------:|-----------:|
| Applying1EventWithNothingToReplay         |    87.24 ms |  30.47 ms | 20.16 ms |    79.80 ms |    74.47 ms |   139.9 ms |           - |          - |     317 KB |
| Applying100EventsWithNothingToReplay      |   565.82 ms | 104.78 ms | 69.31 ms |   543.03 ms |   475.46 ms |   706.2 ms |   2000.0000 |  1000.0000 |   8,785 KB |
| Applying1Event100TimesWithNothingToReplay | 4,231.52 ms |  95.11 ms | 62.91 ms | 4,232.51 ms | 4,128.10 ms | 4,306.0 ms | 124000.0000 | 41000.0000 | 341,919 KB |
