// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common;
using Dolittle.SDK.Common.ClientSetup;

namespace Dolittle.SDK.Events.Handling.Builder.Convention.Instance;

/// <summary>
/// Represents an implementation of <see cref="ClientUniqueDecoratedBindingsBuilder{TIdentifier,TValue,TUniqueBindings,TDecorator}"/> for building convention instance event handlers.
/// </summary>
public class ConventionInstanceEventHandlerBindingsBuilder : ClientUniqueDecoratedBindingsBuilder<EventHandlerId, object, IConventionInstanceEventHandlers, EventHandlerAttribute>
{
    /// <inheritdoc />
    protected override IConventionInstanceEventHandlers CreateUniqueBindings(IClientBuildResults aggregatedBuildResults, IUniqueBindings<EventHandlerId, object> bindings)
        => new ConventionInstanceEventHandlers(bindings);

    /// <inheritdoc />
    protected override bool TryGetIdentifierFromDecorator(object decoratedType, EventHandlerAttribute attribute, out EventHandlerId identifier)
    {
        identifier = attribute.Identifier;
        return true;
    }
}
