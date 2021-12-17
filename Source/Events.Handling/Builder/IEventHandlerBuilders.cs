// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common;

namespace Dolittle.SDK.Events.Handling.Builder;

/// <summary>
/// Defines the unique event handler builder bindings.
/// </summary>
public interface IEventHandlerBuilders : IUniqueBindings<EventHandlerId, EventHandlerBuilder>, ICanBuildEventHandlerBindings
{
}
