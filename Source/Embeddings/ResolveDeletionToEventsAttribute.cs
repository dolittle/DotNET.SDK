// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Embeddings;

/// <summary>
/// Decorates a method to indicate that it's an embeddings remove method.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ResolveDeletionToEventsAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResolveDeletionToEventsAttribute"/> class.
    /// </summary>
    public ResolveDeletionToEventsAttribute()
    {
    }
}