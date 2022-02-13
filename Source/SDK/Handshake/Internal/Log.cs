// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Failures;
using Microsoft.Extensions.Logging;
using Version = Dolittle.SDK.Microservices.Version;

namespace Dolittle.SDK.Handshake.Internal;

/// <summary>
/// Log messages for <see cref="Dolittle.SDK.Handshake.Internal"/>.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "Performing handshake between Head v{HeadVersion} using Dolittle DotNET SDK v{SDKVersion} using version {ContractsVersion} Contracts and Dolittle Runtime")]
    internal static partial void PerformHandshake(this ILogger logger, Version headVersion, Version sdkVersion, Version contractsVersion);

    [LoggerMessage(0, LogLevel.Debug, "Error performing handshake with Dolittle Runtime")]
    internal static partial void ErrorPerformingHandshake(this ILogger logger, Exception ex);

    [LoggerMessage(0, LogLevel.Warning, "Dolittle Runtime failed performing handshake. ({FailureId}){FailureReason}")]
    internal static partial void HandshakeFailedResponse(this ILogger logger, FailureReason failureReason, FailureId failureId);

    [LoggerMessage(0, LogLevel.Debug, "Successfully performed handshake between Head v{HeadVersion} using Dolittle DotNET SDK v{SDKVersion} using version {SDKContractsVersion} Contracts and Dolittle Runtime v{RuntimeVersion} using version {RuntimeContractsVersion} Contracts")]
    internal static partial void SuccessfullyPerformedHandshake(this ILogger logger, Version headVersion, Version sdkVersion, Version sdkContractsVersion, Version runtimeVersion, Version runtimeContractsVersion);
}
