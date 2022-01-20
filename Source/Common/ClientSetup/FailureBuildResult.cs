// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Common.ClientSetup;

/// <summary>
/// Represents the failure <see cref="ClientBuildResult"/>.
/// </summary>
public class FailureBuildResult : ClientBuildResult
{
    /// <summary>
    /// Initializes an instance of the <see cref="FailureBuildResult"/> class.
    /// </summary>
    /// <param name="message">The build message.</param>
    /// <param name="fix">The fix message.</param>
    public FailureBuildResult(string message, string fix = default) : base(LogLevel.Warning, string.IsNullOrEmpty(fix) ? message : $"{message}. {fix}", true)
    {
    }
}
