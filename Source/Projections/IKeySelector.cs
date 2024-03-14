// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections;

/// <summary>
/// Use this interface to define projection key selectors.
/// Instances of this interface MUST be thread safe and stateless, as they are used as singletons.
/// </summary>
/// <typeparam name="TEvent">The mapped event type</typeparam>
public interface IKeySelector<TEvent> where TEvent : class
{
    /// <summary>
    /// Map to a <see cref="Key"/> from an event and context.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="eventContext"></param>
    /// <returns>The projection key</returns>
    Key Selector(TEvent @event, EventContext eventContext);
}
