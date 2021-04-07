// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Projections.Builder;

namespace Dolittle.SDK.Projections
{
    /// <summary>
    /// Decorates a projection method with the <see cref="KeySelectorType.PartitionId" />.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class KeyFromPartitionAttribute : Attribute, IKeySelectorAttribute
    {
        /// <inheritdoc/>
        public KeySelector KeySelector { get; } = new KeySelectorBuilder().KeyFromPartitionId();
    }
}
