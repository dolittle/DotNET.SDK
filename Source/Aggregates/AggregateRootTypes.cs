// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Artifacts;
using Dolittle.SDK.Common;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates;

/// <summary>
/// Represents an implementation of <see cref="IAggregateRootTypes" />.
/// </summary>
public class AggregateRootTypes : Artifacts<AggregateRootType, AggregateRootId>, IAggregateRootTypes
{
    /// <summary>
    /// Initializes an instance of the <see cref="AggregateRootTypes"/> class.
    /// </summary>
    /// <param name="bindings">The <see cref="AggregateRootType"/> associations.</param>
    public AggregateRootTypes(IUniqueBindings<AggregateRootType, Type> bindings)
        : base(bindings)
    {
    }

    /// <summary>
    /// Initializes an instance of the <see cref="AggregateRootTypes"/> class.
    /// </summary>
    public AggregateRootTypes()
    {
    }
}
