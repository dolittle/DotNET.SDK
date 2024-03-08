// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using Dolittle.SDK.Execution;
using Dolittle.SDK.Microservices;
using Dolittle.SDK.Security;
using Dolittle.SDK.Tenancy;

namespace Dolittle.SDK.Testing;

public static class ExecutionContexts
{
    public static readonly ExecutionContext Test = new(
        MicroserviceId.NotSet,
        TenantId.Development,
        Version.NotSet,
        Environment.Undetermined,
        CorrelationId.System,
        Claims.Empty,
        CultureInfo.InvariantCulture);
}
