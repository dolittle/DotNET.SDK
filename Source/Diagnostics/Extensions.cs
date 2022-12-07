// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenTelemetry.Trace;

namespace Diagnostics;

/// <summary>
/// Used to enable diagnostics for Dolittle SDK
/// </summary>
public static class Extensions
{
    
    /// <summary>
    /// Enable tracing in 
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static TracerProviderBuilder AddDolittleInstrumentation(this TracerProviderBuilder builder)
        => builder.AddSource(Tracing.ActivitySourceName);
}
