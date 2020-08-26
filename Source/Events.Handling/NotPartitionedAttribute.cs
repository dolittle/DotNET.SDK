// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Events.Handling
{
    /// <summary>
    /// Decorates a method to indicate that the decorated Event Handler should not be partitioned.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class NotPartitionedAttribute : Attribute
    {
    }
}