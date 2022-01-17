// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Projections.Store;

/// <summary>
/// Exception that gets thrown when trying to associate a type that isn't a projection.
/// </summary>
public class TypeIsNotAProjection : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeIsNotAProjection"/> class.
    /// </summary>
    /// <param name="type">The type trying to associate.</param>
    public TypeIsNotAProjection(Type type)
        : base($"Type {type} is not a projection. Did you add the [Projection()] attribute to it?")
    {
    }
}