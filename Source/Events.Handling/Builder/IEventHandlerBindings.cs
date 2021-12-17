// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common;
using Dolittle.SDK.Events.Handling.Builder.Convention.Instance;
using Dolittle.SDK.Events.Handling.Builder.Convention.Type;

namespace Dolittle.SDK.Events.Handling.Builder;

/// <summary>
/// Defines a complete set of <see cref="IUniqueBindings{TIdentifier,TValue}"/> binding <see cref="EventHandlerId"/> to <see cref="IEventHandler"/>.
/// </summary>
public interface IEventHandlerBindings : IUniqueBindings<EventHandlerId, IEventHandler>
{
    /// <summary>
    /// Gets the <see cref="IConventionInstanceEventHandlers"/>.
    /// </summary>
    IConventionInstanceEventHandlers Instances { get; }
    
    /// <summary>
    /// Gets the <see cref="IConventionTypeEventHandlers"/>.
    /// </summary>
    IConventionTypeEventHandlers Typed { get; }

}
