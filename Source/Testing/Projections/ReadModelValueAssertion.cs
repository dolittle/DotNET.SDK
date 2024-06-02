// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Testing.Aggregates.Events;

namespace Dolittle.SDK.Testing.Projections;

/// <summary>
/// Fluent interface element allowing assertions against a specific event.
/// </summary>
/// <typeparam name="T">The type of the event you wish to assert against.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="EventValueAssertion{T}"/> class with the event to assert against.
/// </remarks>
/// <param name="model">The event you wish to assert against.</param>
public class ReadModelValueAssertion<T>(T model)
    where T : class
{
    /// <summary>
    /// Gets the event that is being asserted against.
    /// </summary>
    public T ReadModel { get; } = model;

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
            assert(ReadModel);
        }
    }
}
