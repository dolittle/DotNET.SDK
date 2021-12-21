// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common;

namespace Dolittle.SDK.Events.Handling.Builder.Convention.Instance;

/// <summary>
/// Represents an implementation of <see cref="IConventionInstanceEventHandlers"/>.
/// </summary>
public class ConventionInstanceEventHandlers : UniqueBindings<EventHandlerId, object>, IConventionInstanceEventHandlers
{
    /// <summary>
    /// Initializes an instance of the <see cref="ConventionInstanceEventHandlers"/> class. 
    /// </summary>
    /// <param name="bindings">The convention instance event handler bindings.</param>
    public ConventionInstanceEventHandlers(IUniqueBindings<EventHandlerId, object> bindings)
        : base(bindings)
    {
    }
}
