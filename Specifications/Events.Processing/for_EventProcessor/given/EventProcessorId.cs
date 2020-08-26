// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Concepts;

namespace Dolittle.Events.Processing.for_EventProcessor.given
{
    public class EventProcessorId : ConceptAs<Guid>
    {
        public static implicit operator EventProcessorId(string id) => new EventProcessorId { Value = Guid.Parse(id) };
    }
}