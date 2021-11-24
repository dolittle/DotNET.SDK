// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.SDK.Events.Store;

namespace Dolittle.SDK.Testing.Aggregates
{
    public class EventStoreMock : IEventStore
    {
        public IList<object> AlreadyCommittedEvents { get; }
        
        public IList<object> CommittedEvents { get; }
    }
}