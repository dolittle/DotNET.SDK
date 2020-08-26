// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Commands.Handling;
using Dolittle.Domain;

namespace EventSourcing
{
    public class MyCommandHandler : ICanHandleCommands
    {
        readonly IAggregateOf<MyAggregate> _aggregateOf;

        public MyCommandHandler(IAggregateOf<MyAggregate> aggregateOf)
        {
            _aggregateOf = aggregateOf;
        }

        public void Handle(MyCommand command)
        {
            _aggregateOf.Create().Perform(_ => _.DoStuff());
        }
    }
}
