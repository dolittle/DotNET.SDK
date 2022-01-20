// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Failures;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Tenancy.Client.Internal;

static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "Getting all Tenants")]
    internal static partial void GettingAllTenants(ILogger logger);
    
    [LoggerMessage(0, LogLevel.Warning, "An error occurred while getting all Tenants because {FailureReason}. Failure Id '{FailureId}'")]
    internal static partial void FailedGettingAllTenants(ILogger logger, FailureReason failureReason, FailureId failureId);
    
    [LoggerMessage(0, LogLevel.Warning, "An error occurred while getting all Tenants")]
    internal static partial void ErrorGettingTenants(ILogger logger, Exception ex);
}
