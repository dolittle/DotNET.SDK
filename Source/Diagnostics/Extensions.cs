// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenTelemetry.Trace;

namespace Dolittle.SDK.Diagnostics;

/// <summary>
/// Used to enable diagnostics for Dolittle SDK
/// </summary>
public static class Extensions
{
    const string MongodbDriverActivitySource = "MongoDB.Driver.Core.Extensions.DiagnosticSources";

    /// <summary>
    /// Enable tracing for Dolittle SDK
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static TracerProviderBuilder AddDolittleInstrumentation(this TracerProviderBuilder builder)
        => builder.AddSource(Tracing.ActivitySourceName);
    
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Enable tracing for MongoDB
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static TracerProviderBuilder AddMongoDBInstrumentation(this TracerProviderBuilder builder)
        => builder.AddSource(MongodbDriverActivitySource);
    
}
