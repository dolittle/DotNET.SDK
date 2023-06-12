// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.Metrics;

namespace Dolittle.SDK.Diagnostics.OpenTelemetry;

/// <summary>
/// Represents the metrics for the Dolittle SDK.
/// </summary>
public static class Metrics
{
    /// <summary>
    /// The name of the meter.
    /// </summary>
    public const string MeterName = "dolittle-sdk";

    static readonly Meter _meterProvider = new(MeterName);

    static readonly Histogram<double> _eventsProcessTime = _meterProvider.CreateHistogram<double>(
        "dolittle.events-processed",
        "seconds",
        "Duration of events processed");

    static readonly Counter<int> _eventsCommitted = _meterProvider.CreateCounter<int>(
        "dolittle.events-committed",
        "events",
        "Number of events committed");

    static readonly Counter<long> _bytesCommitted = _meterProvider.CreateCounter<long>(
        "dolittle.events-committed-bytes",
        "byte",
        "Total size of committed events in bytes");

    static readonly Histogram<double> _failingEventProcessTime = _meterProvider.CreateHistogram<double>(
        "dolittle.events-failed-processing",
        "seconds",
        "Durations of event that failed processing");

    static readonly Counter<long> _eventsRehydrated = _meterProvider.CreateCounter<long>(
        "dolittle.events-read",
        "events",
        "Total number of events replayed");

    static readonly Counter<long> _eventBytesRehydrated = _meterProvider.CreateCounter<long>(
        "dolittle.events-read-bytes",
        "byte",
        "Total number bytes replayed. Combine with number of events for average size of events");

    /// <summary>
    /// Register event processed successfully.
    /// </summary>
    /// <param name="duration"></param>
    public static void EventProcessed(TimeSpan duration) => _eventsProcessTime.Record(duration.TotalSeconds);

    /// <summary>
    /// Register event processed unsuccessfully.
    /// </summary>
    /// <param name="duration"></param>
    public static void EventFailedToProcess(TimeSpan duration) => _failingEventProcessTime.Record(duration.TotalSeconds);


    /// <summary>
    /// Register event committed.
    /// </summary>
    public static void EventsCommitted(int events, long totalBytes)
    {
        _eventsCommitted.Add(events);
        _bytesCommitted.Add(totalBytes);
    }

    /// <summary>
    /// Register events used to rehydrate aggregates.
    /// </summary>
    public static void EventsRehydrated(int events, long totalBytes)
    {
        _eventsRehydrated.Add(events);
        _eventBytesRehydrated.Add(totalBytes);
    }
}
