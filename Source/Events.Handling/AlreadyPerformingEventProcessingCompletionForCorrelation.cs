// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Execution;

namespace Dolittle.Events.Handling
{
    /// <summary>
    /// Exception that gets thrown when <see cref="EventProcessingCompletion.Perform(CorrelationId, IEnumerable{IEvent}, Action)" /> is already performing for a <see cref="CorrelationId" />.
    /// </summary>
    public class AlreadyPerformingEventProcessingCompletionForCorrelation : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlreadyPerformingEventProcessingCompletionForCorrelation"/> class.
        /// </summary>
        /// <param name="correlation">The <see cref="CorrelationId" />.</param>
        public AlreadyPerformingEventProcessingCompletionForCorrelation(CorrelationId correlation)
            : base($"Already performing event processing completion for command with correlation {correlation}")
        {
        }
    }
}
