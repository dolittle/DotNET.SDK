// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Tenancy;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Resources.Internal;

/// <summary>
/// Log messages for <see cref="Dolittle.SDK.Resources.Internal"/>.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "Getting {ResourceName} resource for tenant {Tenant}")]
    internal static partial void GettingResource(this ILogger logger, ResourceName resourceName, TenantId tenant);
    
    [LoggerMessage(0, LogLevel.Warning, "Failed getting {ResourceName} resource for tenant {Tenant} because {Reason}")]
    internal static partial void FailedToGetResource(this ILogger logger, ResourceName resourceName, TenantId tenant, string reason);
    
    [LoggerMessage(0, LogLevel.Warning, "An error occured while getting {ResourceName} resource for tenant {Tenant}")]
    internal static partial void ErrorWhileGettingResource(this ILogger logger, ResourceName resourceName, TenantId tenant, Exception exception);
}
