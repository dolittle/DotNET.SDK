// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common;

namespace Dolittle.SDK.Events.Handling.Builder.Convention.Type;

/// <summary>
/// Represents an implementation of <see cref="IConventionTypeEventHandlers"/>.
/// </summary>
public class ConventionTypeEventHandlers : UniqueBindings<EventHandlerId, System.Type>, IConventionTypeEventHandlers
{
    /// <summary>
    /// Initializes an instance of the <see cref="ConventionTypeEventHandlers"/> class.
    /// </summary>
    /// <param name="bindings"></param>
    public ConventionTypeEventHandlers(IUniqueBindings<EventHandlerId, System.Type> bindings)
        : base(bindings)
    {
    }
}
