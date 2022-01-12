// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Common;

namespace Dolittle.SDK.Events;

/// <summary>
/// Represents an implementation of <see cref="IEventTypes" />.
/// </summary>
public class EventTypes : Artifacts<EventType, EventTypeId>, IEventTypes
{
    /// <summary>
    /// Initializes an instance of the <see cref="EventTypes"/> class.
    /// </summary>
    /// <param name="bindings">The <see cref="IUniqueBindings{TIdentifier,TValue}"/> for "/><see cref="EventType"/> to <see cref="Type"/>.</param>
    public EventTypes(IUniqueBindings<EventType, Type> bindings)
        : base(bindings)
    {
    }
}
