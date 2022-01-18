// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections;

/// <summary>
/// Exception that gets thrown when an projection on-method failed handling an event.
/// </summary>
public class ProjectionOnMethodFailed : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionOnMethodFailed"/> class.
    /// </summary>
    /// <param name="projection">The <see cref="ProjectionId" />.</param>
    /// <param name="eventType">The <see cref="EventType" />.</param>
    /// <param name="event">The event that failed handling.</param>
    /// <param name="exception">The <see cref="Exception" /> that caused the handling to fail.</param>
    public ProjectionOnMethodFailed(ProjectionId projection, EventType eventType, object @event, Exception exception)
        : base($"Projection {projection} failed to handle event {@event} with event type {eventType} ", exception)
    {
    }
}