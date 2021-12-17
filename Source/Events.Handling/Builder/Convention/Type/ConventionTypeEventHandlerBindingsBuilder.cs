// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common;
using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Events.Handling.Builder.Convention.Type;

/// <summary>
/// Represents an implementation of <see cref="ClientUniqueDecoratedBindingsBuilder{TIdentifier,TValue,TUniqueBindings,TDecorator}"/> for building convention type event handlers.
/// </summary>
public class ConventionTypeEventHandlerBindingsBuilder : ClientUniqueDecoratedBindingsBuilder<EventHandlerId, System.Type, IConventionTypeEventHandlers, EventHandlerAttribute>
{
    /// <inheritdoc />
    protected override IConventionTypeEventHandlers CreateUniqueBindings(IClientBuildResults aggregatedBuildResults, IUniqueBindings<EventHandlerId, System.Type> bindings)
        => new ConventionTypeEventHandlers(bindings);

    /// <inheritdoc />
    protected override bool TryGetIdentifierFromDecorator(System.Type decoratedType, EventHandlerAttribute attribute, out EventHandlerId identifier)
    {
        identifier = attribute.Identifier;
        return true;
    }
}
