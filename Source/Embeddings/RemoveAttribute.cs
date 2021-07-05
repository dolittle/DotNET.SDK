// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Embeddings
{
    /// <summary>
    /// Decorates a method to indicate that it's an embeddings remove method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RemoveAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveAttribute"/> class.
        /// </summary>
        public RemoveAttribute()
        {
        }
    }
}
