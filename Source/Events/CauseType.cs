// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Events
{
    /// <summary>
    /// Represents the types of causes that can cause <see cref="IEvent"/>s to occur.
    /// </summary>
    public enum CauseType
    {
        /// <summary>Indicates that the <see cref="IEvent" /> has an unknown cause.</summary>
        Unknonwn = 0,

        /// <summary>Indicates that the <see cref="IEvent"/> was caused by a Command.</summary>
        Command = 1,

        /// <summary>Indicates that the <see cref="IEvent"/> was caused by an <see cref="IEvent"/>.</summary>
        Event = 2,
    }
}
