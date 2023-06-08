// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Common.Model;

namespace Dolittle.SDK.Events.Handling;

/// <summary>
/// Represents the identifier of an event handler in an application model.
/// </summary>
public class EventHandlerModelId : Identifier<EventHandlerId, ScopeId>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlerModelId"/> class.
    /// </summary>
    /// <param name="id">The <see cref="EventHandlerId"/>.</param>
    /// <param name="partitioned">The value indicating whether the event handler is partitioned.</param>
    /// <param name="scope">The <see cref="ScopeId"/>.></param>
    /// <param name="alias">The alias.</param>
    /// <param name="concurrency"></param>
    /// <param name="resetTo"></param>
    /// <param name="startFrom"></param>
    /// <param name="stopAt"></param>
    public EventHandlerModelId(
        EventHandlerId id,
        bool partitioned,
        ScopeId scope,
        string? alias,
        int concurrency = 1,
        ProcessFrom resetTo = ProcessFrom.First,
        DateTimeOffset? startFrom = null,
        DateTimeOffset? stopAt = null)
        : base("EventHandler", id, alias, scope)
    {
        if (startFrom is not null && stopAt is not null && startFrom > stopAt)
        {
            throw new ArgumentException("StartFrom cannot be after StopAt");
        }

        Scope = scope;
        Concurrency = concurrency < 1 ? 1 : concurrency;
        ResetTo = resetTo;
        StartFrom = startFrom;
        StopAt = stopAt;
        Partitioned = partitioned;
    }

    /// <summary>
    /// Gets whether the event handler is partitioned.
    /// </summary>
    public bool Partitioned { get; }

    /// <summary>
    /// Gets the <see cref="ScopeId"/>.
    /// </summary>
    public ScopeId Scope { get; }

    public int Concurrency { get; }
    public ProcessFrom ResetTo { get; }
    public DateTimeOffset? StartFrom { get; }
    public DateTimeOffset? StopAt { get; }
}
