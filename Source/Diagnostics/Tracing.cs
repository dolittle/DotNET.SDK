// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace Dolittle.SDK.Diagnostics;

/// <summary>
/// Diagnostics constants
/// </summary>
public static class Tracing
{
    /// <summary>
    /// Activity source name
    /// </summary>
    public const string ActivitySourceName = "dolittle-sdk";

    /// <summary>
    /// Dolittle SDK ActivitySource
    /// </summary>
    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);
}
