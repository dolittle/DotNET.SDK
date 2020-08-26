// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Contracts;

namespace Dolittle.Events.Processing.for_EventProcessor.given
{
    public class Response
    {
        public Request Request { get; set; }

        public ProcessorFailure Failure { get; set; }
    }
}