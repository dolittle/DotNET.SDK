// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Execution;

namespace Dolittle.SDK.Events.Store
{
    /// <summary>
    /// Exception that gets thrown when trying to construct a <see cref="CommittedEvent"/> without an <see cref="ExecutionContext"/>.
    /// </summary>
    public class ExecutionContextCannotBeNull : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionContextCannotBeNull"/> class.
        /// </summary>
        public ExecutionContextCannotBeNull()
            : base($"The {nameof(ExecutionContext)} of an {nameof(CommittedEvent)} cannot be null")
        {
        }
    }
}