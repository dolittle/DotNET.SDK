// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Failures.Events;

namespace Dolittle.SDK.Failures
{
    /// <summary>
    /// Represents the known failures that can be returned from the Runtime, and the mappings to corresponding types of <see cref="Exception"/>.
    /// </summary>
    public static class Exceptions
    {
        static readonly IDictionary<FailureId, Func<string, Exception>> _exceptionMappings = new Dictionary<FailureId, Func<string, Exception>>
        {
            // Event Store failures
            { "b6fcb5dd-a32b-435b-8bf4-ed96e846d460", _ => new EventStoreUnavailable(_) },
            { "d08a30b0-56ab-43dc-8fe6-490320514d2f", _ => new EventWasAppliedByOtherAggregateRoot(_) },
            { "b2acc526-ba3a-490e-9f15-9453c6f13b46", _ => new EventWasAppliedToOtherEventSource(_) },
            { "ad55fca7-476a-4f68-9411-1a3b087ab843", _ => new EventStorePersistenceError(_) },
            { "6f0e6cab-c7e5-402e-a502-e095f9545297", _ => new EventStoreConsistencyError(_) },
            { "eb508238-87ff-4519-a743-03be5196a83d", _ => new EventLogSequenceIsOutOfOrder(_) },
            { "45a811d9-bdf7-4ee1-b9bc-3f248e761799", _ => new EventCanNotBeNull(_) },
            { "eb51284e-c7b4-4966-8da4-64a862f07560", _ => new AggregateRootVersionIsOutOfOrder(_) },
            { "f25cccfb-3ae1-4969-bee6-906370ffbc2d", _ => new AggregateRootConcurrencyConflict(_) },
            { "ef3f1a42-9bc3-4d98-aa2a-942db7c56ac1", _ => new NoEventsToCommit(_) },
        };

        /// <summary>
        /// Creates an <see cref="Exception"/> from a <see cref="Failure"/>.
        /// </summary>
        /// <param name="failure">The failure to map to an exception.</param>
        /// <returns>A known kind of <see cref="Exception"/>, or <see cref="UnknownRuntimeFailure"/> if the failure is not known.</returns>
        public static Exception CreateFromFailure(Failure failure)
        {
            if (_exceptionMappings.TryGetValue(failure.Id, out var constructor))
            {
                return constructor(failure.Reason);
            }

            return new UnknownRuntimeFailure(failure);
        }
    }
}
