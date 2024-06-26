﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Testing.Aggregates.Events;

/// <summary>
/// Fluent interface element allowing assertions against a specific event.
/// </summary>
/// <typeparam name="T">The type of the event you wish to assert against.</typeparam>
public class EventValueAssertion<T>
    where T : class
{
    /// <summary>
    /// Gets the event that is being asserted against.
    /// </summary>
    public T Event { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventValueAssertion{T}"/> class with the event to assert against.
    /// </summary>
    /// <param name="evt">The event you wish to assert against.</param>
    public EventValueAssertion(T evt)
    {
        Event = evt;
    }

    /// <summary>
    /// Asserts that the event passes the specified assertions.
    /// </summary>
    /// <param name="assertions">>A collection of assertions that you wish to perform.</param>
    public void Where(params Action<T>[] assertions) => AndThat(assertions);
    
    /// <summary>
    /// Asserts that the event passes the specified assertions.
    /// </summary>
    /// <param name="assertions">>A collection of assertions that you wish to perform.</param>
    public void AndThat(params Action<T>[] assertions)
    {
        foreach (var assert in assertions)
        {
            assert(Event);
        }
    }
}
