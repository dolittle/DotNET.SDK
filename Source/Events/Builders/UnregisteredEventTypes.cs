// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.SDK.Events.Internal;

namespace Dolittle.SDK.Events.Builders;

/// <summary>
/// Represents an implementation of <see cref="IUnregisteredEventTypes"/>.
/// </summary>
public class UnregisteredEventTypes : EventTypes, IUnregisteredEventTypes
{
    /// <inheritdoc />
    public async Task<IEventTypes> Register(EventTypesClient eventTypesClientClient, CancellationToken cancellationToken)
    {
        await eventTypesClientClient.Register(All, cancellationToken).ConfigureAwait(false);
        return this;
    }
}
