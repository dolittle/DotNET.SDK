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
/// <remarks>
/// Initializes an instance of the <see cref="UnregisteredEventTypes"/> class.
/// </remarks>
/// <param name="bindings">The <see cref="IUniqueBindings{TIdentifier,TValue}"/> for <see cref="EventType"/> to <see cref="Type"/>.</param>
public class UnregisteredEventTypes(IUniqueBindings<EventType, Type> bindings) : EventTypes(bindings), IUnregisteredEventTypes
{
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
