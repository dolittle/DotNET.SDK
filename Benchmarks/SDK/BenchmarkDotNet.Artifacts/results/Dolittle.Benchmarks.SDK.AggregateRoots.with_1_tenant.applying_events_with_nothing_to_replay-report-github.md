``` ini

BenchmarkDotNet=v0.13.1, OS=macOS Monterey 12.1 (21C52) [Darwin 21.2.0]
Apple M1 Max, 1 CPU, 10 logical and 10 physical cores
.NET SDK=6.0.101
  [Host]     : .NET 6.0.1 (6.0.121.56705), Arm64 RyuJIT
  Job-SGYUYI : .NET 6.0.1 (6.0.121.56705), Arm64 RyuJIT

PowerPlanMode=00000000-0000-0000-0000-000000000000  InvocationCount=1  IterationCount=10  
LaunchCount=1  RunStrategy=ColdStart  UnrollFactor=1  
WarmupCount=0  

```

| Method                                    |      Mean |     Error |    StdDev |    Median |       Min |       Max |      Gen 0 |     Gen 1 | Allocated |
|-------------------------------------------|----------:|----------:|----------:|----------:|----------:|----------:|-----------:|----------:|----------:|
| Applying1EventWithNothingToReplay         |  46.26 ms | 11.168 ms |  7.387 ms |  43.44 ms |  41.68 ms |  65.61 ms |          - |         - |     46 KB |
| Applying100EventsWithNothingToReplay      |  84.65 ms |  9.859 ms |  6.521 ms |  82.50 ms |  77.91 ms |  99.88 ms |          - |         - |    882 KB |
| Applying1Event100TimesWithNothingToReplay | 867.42 ms | 21.583 ms | 14.276 ms | 860.94 ms | 853.53 ms | 895.76 ms | 16000.0000 | 1000.0000 | 34,213 KB |
