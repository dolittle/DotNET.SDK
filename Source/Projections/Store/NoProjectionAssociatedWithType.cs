// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Projections.Store;

/// <summary>
/// Exception that gets thrown when a type has no projection associated with it.
/// </summary>
public class NoProjectionAssociatedWithType : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoProjectionAssociatedWithType"/> class.
    /// </summary>
    /// <param name="type">The type without a projection.</param>
    public NoProjectionAssociatedWithType(Type type)
        : base($"No projection associated with type {type}")
    {
    }
}