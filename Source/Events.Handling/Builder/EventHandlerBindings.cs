// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common;
using Dolittle.SDK.Events.Handling.Builder.Convention.Instance;
using Dolittle.SDK.Events.Handling.Builder.Convention.Type;

namespace Dolittle.SDK.Events.Handling.Builder;

/// <summary>
/// Represents an implementation of <see cref="IEventHandlerBindings"/>.
/// </summary>
public class EventHandlerBindings : UniqueBindings<EventHandlerId, IEventHandler>, IEventHandlerBindings
{
    /// <summary>
    /// Initializes an instance of the <see cref="EventHandlerBindings"/> class.
    /// </summary>
    /// <param name="typed"></param>
    /// <param name="instances"></param>
    /// <param name="allBindings"></param>
    public EventHandlerBindings(IConventionTypeEventHandlers typed, IConventionInstanceEventHandlers instances, IUniqueBindings<EventHandlerId, IEventHandler> allBindings)
        : base(allBindings)
    {
        Typed = typed;
        Instances = instances;
    }

    /// <inheritdoc />
    public IConventionInstanceEventHandlers Instances { get; }

    /// <inheritdoc />
    public IConventionTypeEventHandlers Typed { get; }
}
