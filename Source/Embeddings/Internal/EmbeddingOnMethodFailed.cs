// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Embeddings.Internal;

/// <summary>
/// Exception that gets thrown when an embedding on-method failed handling an event.
/// </summary>
public class EmbeddingOnMethodFailed : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddingOnMethodFailed"/> class.
    /// </summary>
    /// <param name="embedding">The <see cref="EmbeddingId" />.</param>
    /// <param name="eventType">The <see cref="EventType" />.</param>
    /// <param name="event">The event that failed handling.</param>
    /// <param name="exception">The <see cref="Exception" /> that caused the handling to fail.</param>
    public EmbeddingOnMethodFailed(EmbeddingId embedding, EventType eventType, object @event, Exception exception)
        : base($"Embedding {embedding} failed to handle event {@event} with event type {eventType} ", exception)
    {
    }
}