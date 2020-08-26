// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Dolittle.Execution;
using Dolittle.Security;
using Dolittle.Tenancy;
using Environment = Dolittle.Execution.Environment;
using Version = Dolittle.Versioning.Version;

namespace Dolittle.Events.Handling
{
    public static class committed_events
    {
        public static CommittedEvent single() => new CommittedEvent(
                EventLogSequenceNumber.Initial,
                DateTimeOffset.UtcNow,
                EventSourceId.New(),
                new ExecutionContext(
                    ApplicationModel.Microservice.New(),
                    TenantId.Unknown,
                    Version.NotSet,
                    Environment.Undetermined,
                    CorrelationId.New(),
                    Claims.Empty,
                    CultureInfo.InvariantCulture),
                new MyFirstEvent());
    }
}