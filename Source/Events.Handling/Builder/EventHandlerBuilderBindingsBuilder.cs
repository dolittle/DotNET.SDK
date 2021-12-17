// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common;
using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Events.Handling.Builder.Convention.Type;

/// <summary>
/// Represents an <see cref="ClientUniqueBindingsBuilder{TIdentifier,TValue,TUniqueBindings}"/> for building event handler builder bindings.
/// </summary>
public class EventHandlerBuilderBindingsBuilder : ClientUniqueBindingsBuilder<EventHandlerId, EventHandlerBuilder, IEventHandlerBuilders>
{
    /// <inheritdoc />
    protected override IEventHandlerBuilders CreateUniqueBindings(IClientBuildResults aggregatedBuildResults, IUniqueBindings<EventHandlerId, EventHandlerBuilder> bindings)
        => new EventHandlerBuilders(bindings);
}
