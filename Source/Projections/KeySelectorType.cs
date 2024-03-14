// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Projections;

/// <summary>
/// Defines the ways of getting the projection key from an event.
/// </summary>
public enum KeySelectorType
{
    /// <summary>
    /// Gets the key from the event source id metadata.
    /// </summary>
    EventSourceId = 0,

    /// <summary>
    /// Gets the key from the partition id metadata.
    /// </summary>
    PartitionId,

    /// <summary>
    /// Gets the key from a named property on the event content.
    /// </summary>
    Property,
    
    /// <summary>
    /// Gets the from a static key.
    /// </summary>
    Static,
    
    /// <summary>
    /// Gets the key from the event occurred metadata.
    /// </summary>
    EventOccurred,
    
    /// <summary>
    /// Gets the key from a named property on the event content and the event occurred metadata.
    /// </summary>
    PropertyAndEventOccurred,
    
    /// <summary>
    /// Gets the key Gets the key from the event source id and the event occurred metadata.
    /// </summary>
    EventSourceIdAndOccurred,
    
    /// <summary>
    /// The key is returned as a function of the event and the event context.
    /// </summary>
    Function
}
