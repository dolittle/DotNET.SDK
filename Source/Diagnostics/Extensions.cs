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
