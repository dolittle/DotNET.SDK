// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Contracts;

namespace Dolittle.SDK.Projections
{
    /// <summary>
    /// Extensions for <see cref="KeySelectorType" />.
    /// </summary>
    public static class KeySelectorTypeExtensions
    {
        /// <summary>
        /// Converts <see cref="KeySelectorType" /> to <see cref="ProjectionEventKeySelectorType" />.
        /// </summary>
        /// <param name="keySelectorType">The <see cref="KeySelectorType" />.</param>
        /// <returns>The converted <see cref="ProjectionEventKeySelectorType" />.</returns>
        public static ProjectionEventKeySelectorType ToProtobuf(this KeySelectorType keySelectorType)
        {
            return keySelectorType switch
            {
                KeySelectorType.EventSourceId => ProjectionEventKeySelectorType.EventSourceId,
                KeySelectorType.PartitionId => ProjectionEventKeySelectorType.PartitionId,
                KeySelectorType.Property => ProjectionEventKeySelectorType.Property,
                _ => throw new UnknownKeySelectorType(keySelectorType)
            };
        }
    }
}
