// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Reports;

namespace Dolittle.Benchmarks.SDK;

public static class SummaryExtensions
{
    public static int ToExitCode(this IEnumerable<Summary> summaries)
    {
        var enumerable = summaries as Summary[] ?? summaries.ToArray();
        if (!enumerable.Any())
        {
            return 1;
        }
        return enumerable.Any(summary => summary.HasCriticalValidationErrors || summary.Reports.Any(report => !report.BuildResult.IsBuildSuccess || !report.AllMeasurements.Any()))
            ? 1
            : 0;
    }
}
