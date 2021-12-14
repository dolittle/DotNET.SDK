// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events.Internal;

namespace Dolittle.SDK.Events.Builders;

/// <summary>
/// Represents an implementation of <see cref="IUnregisteredEventTypes"/>.
/// </summary>
public class UnregisteredEventTypes : EventTypes, IUnregisteredEventTypes
{
    /// <summary>
    /// Initializes an instance of the <see cref="UnregisteredEventTypes"/> class.
    /// </summary>
    /// <param name="associations">The <see cref="EventType"/> associations.</param>
    public UnregisteredEventTypes(IDictionary<Type, EventType> associations) : base(associations)
    {
    }

    /// <inheritdoc />
    public async Task<IEventTypes> Register(EventTypesClient eventTypesClientClient, CancellationToken cancellationToken)
    {
        await eventTypesClientClient.Register(All, cancellationToken).ConfigureAwait(false);
        return this;
    }
}
