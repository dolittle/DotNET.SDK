// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing.Contracts;

namespace Dolittle.SDK.Projections.Store.Converters
{
    /// <summary>
    /// Exception that gets thrown when converting projection to SDK and <see cref="ProjectionCurrentState.Type" /> is invalid.
    /// </summary>
    public class InvalidCurrentStateType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCurrentStateType"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ProjectionCurrentStateType" />.</param>
        public InvalidCurrentStateType(ProjectionCurrentStateType type)
            : base($"Could not convert {nameof(ProjectionCurrentStateType)} because {type} is an invalid type")
        {
        }
    }
}
