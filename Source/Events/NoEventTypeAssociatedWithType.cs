// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events
{
    /// <summary>
    /// Exception that gets thrown when a <see cref="Type" /> does not have an <see cref="EventType"/> association.
    /// </summary>
    public class NoEventTypeAssociatedWithType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoEventTypeAssociatedWithType"/> class.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> that has a missing association.</param>
        public NoEventTypeAssociatedWithType(Type type)
            : base($"{type} is not associated with an EventType")
        {
        }
    }
}
