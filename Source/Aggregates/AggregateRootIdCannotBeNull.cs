// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Aggregates;

/// <summary>
/// Exception that gets thrown when trying to construct an <see cref="AggregateRootType"/> without an <see cref="AggregateRootId"/>.
/// </summary>
public class AggregateRootIdCannotBeNull : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootIdCannotBeNull"/> class.
    /// </summary>
    public AggregateRootIdCannotBeNull()
        : base($"The {nameof(AggregateRootId)} of an {nameof(AggregateRootType)} cannot be null")
    {
    }
}