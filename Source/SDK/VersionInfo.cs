// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Microservices;

namespace Dolittle.SDK;

/// <summary>
/// Represents a container for the Dolittle DotNET SDK version.
/// </summary>
public static class VersionInfo
{
    /// <summary>
    /// Gets the current <see cref="Version"/> of the Dolittle DotNET SDK.
    /// </summary>
    public static Version CurrentVersion => new(377, 389, 368, 0, "PRERELEASE");
}