// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Dolittle.SDK.Common.Model;

namespace Dolittle.SDK.Events.Handling;

/// <summary>
/// Decorates a class to indicate the Event Handler Id of the Event Handler class.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class EventHandlerAttribute : Attribute, IDecoratedTypeDecorator<EventHandlerModelId>
{
    readonly EventHandlerId _eventHandlerId;
    readonly string? _alias;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlerAttribute"/> class.
    /// </summary>
    /// <param name="eventHandlerId">The unique identifier of the event handler.</param>
    /// <param name="partitioned">Whether the event handler is partitioned.</param>
    /// <param name="inScope">The scope that the event handler handles events in.</param>
    /// <param name="alias">The alias for the event handler.</param>
    /// <param name="concurrency">How many events can be processed simultaneously</param>
    /// <param name="startFrom">Where in the event log to start processing if the event handler has not been used before. Defaults to the first event.</param>
    /// <param name="startFromTimestamp">Determines a specific event timestamp to start at if set. Overrides startFrom when used. Format is ISO 8601</param>
    /// <param name="stopAtTimestamp">Determines a specific event timestamp to stop processing at if set. Format is ISO 8601</param>
    public EventHandlerAttribute(
        string eventHandlerId,
        bool partitioned = true,
        string? inScope = null,
        string? alias = null,
        int concurrency = 1,
        ProcessFrom startFrom = ProcessFrom.Earliest,
        string? startFromTimestamp = null,
        string? stopAtTimestamp = null
    )
    {
        _eventHandlerId = eventHandlerId;
        _alias = alias;
        Concurrency = concurrency;
        StartFrom = startFrom;
        StartFromTimestamp = string.IsNullOrWhiteSpace(startFromTimestamp) ? null : DateTimeOffset.Parse(startFromTimestamp, CultureInfo.InvariantCulture);
        StopAtTimestamp = string.IsNullOrWhiteSpace(stopAtTimestamp) ? null : DateTimeOffset.Parse(stopAtTimestamp, CultureInfo.InvariantCulture);
        if (startFromTimestamp is not null && stopAtTimestamp is not null && StartFromTimestamp >= StopAtTimestamp)
        {
            throw new ArgumentException("StartFromTimestamp must be before StopAtTimestamp");
        }

        Partitioned = partitioned;
        Scope = inScope ?? ScopeId.Default;
    }

    public int Concurrency { get; set; }
    public ProcessFrom StartFrom { get; }
    public DateTimeOffset? StartFromTimestamp { get; }
    public DateTimeOffset? StopAtTimestamp { get; }

    /// <summary>
    /// Gets a value indicating whether this event handler is partitioned.
    /// </summary>
    public bool Partitioned { get; }

    /// <summary>
    /// Gets the <see cref="ScopeId" />.
    /// </summary>
    public ScopeId Scope { get; }


    /// <inheritdoc />
    public EventHandlerModelId GetIdentifier(Type decoratedType)
    {
        // DateTimeOffset? startFrom = null;
        // DateTimeOffset? stopAt = null;
        // if (!string.IsNullOrWhiteSpace(StartFrom))
        // {
        //     startFrom = DateTimeOffset.Parse(StartFrom, DateTimeFormatInfo.InvariantInfo);
        // }
        // if (!string.IsNullOrWhiteSpace(StopAt))
        // {
        //     stopAt = DateTimeOffset.Parse(StopAt, DateTimeFormatInfo.InvariantInfo);
        // }

        return new(_eventHandlerId, Partitioned, Scope, _alias ?? decoratedType.Name, Concurrency, StartFrom, StartFromTimestamp, StopAtTimestamp);
    }
}
