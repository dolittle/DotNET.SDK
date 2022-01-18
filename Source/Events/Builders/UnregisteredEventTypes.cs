// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Common;
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
    /// <param name="bindings">The <see cref="IUniqueBindings{TIdentifier,TValue}"/> for <see cref="EventType"/> to <see cref="Type"/>.</param>
    public UnregisteredEventTypes(IUniqueBindings<EventType, Type> bindings) : base(bindings)
    {
    }

    /// <inheritdoc />
    public async Task<IEventTypes> Register(EventTypesClient eventTypesClientClient, CancellationToken cancellationToken)
    {
        await eventTypesClientClient.Register(Bindings.Select(WithDefaultAliasIfNotSet), cancellationToken).ConfigureAwait(false);
        return this;
    }
    static EventType WithDefaultAliasIfNotSet((EventType, Type) binding)
    {
        var (eventType, type) = binding;
        if (!eventType.HasAlias)
        {
            eventType = new EventType(eventType.Id, eventType.Generation, type.Name);
        }
        return eventType;
    }
}
