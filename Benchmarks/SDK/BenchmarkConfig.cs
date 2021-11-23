// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;

namespace Dolittle.Benchmarks.SDK;

public static class BenchmarkConfig
{
    public static IConfig Create()
    {
        var job = Job.Default
            .WithWarmupCount(0)
            .WithIterationCount(1)
            .WithUnrollFactor(1)
            .WithInvocationCount(1)
            .WithStrategy(RunStrategy.ColdStart)
            .WithLaunchCount(1)
            .DontEnforcePowerPlan();
        
        return ManualConfig.CreateEmpty()
            .AddLogger(ConsoleLogger.Default)
            .AddValidator(DefaultConfig.Instance.GetValidators().ToArray())
            .AddAnalyser(DefaultConfig.Instance.GetAnalysers().ToArray())
            .AddExporter(MarkdownExporter.GitHub)
            .AddColumnProvider(DefaultColumnProviders.Instance)
            .AddJob(job.AsDefault())
            .AddDiagnoser(MemoryDiagnoser.Default)
            .AddExporter(JsonExporter.Full)
            .AddColumn(StatisticColumn.Median, StatisticColumn.Min, StatisticColumn.Max)
            .WithSummaryStyle(SummaryStyle.Default.WithMaxParameterColumnWidth(36));
    }
}