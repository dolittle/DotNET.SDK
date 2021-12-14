// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.SDK.Events.Builders;

/// <summary>
/// Defines a collection of <see cref="EventType"/> that hasn't been registered with the Runtime yet.
/// </summary>
public interface IUnregisteredEventTypes : IEventTypes
{
    /// <summary>
    /// Registers all the <see cref="IEventTypes"/> with the Runtime.
    /// </summary>
    /// <param name="eventTypesClientClient">The <see cref="Internal.EventTypesClient"/> for registering the <see cref="IEventTypes"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<IEventTypes> Register(Internal.EventTypesClient eventTypesClientClient, CancellationToken cancellationToken);
}
