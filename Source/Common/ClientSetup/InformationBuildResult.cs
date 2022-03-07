// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Common.ClientSetup;

/// <summary>
/// Represents the information <see cref="ClientBuildResult"/>.
/// </summary>
public class InformationBuildResult : ClientBuildResult
{
    /// <summary>
    /// Initializes an instance of the <see cref="InformationBuildResult"/> class.
    /// </summary>
    /// <param name="message">The build message.</param>
    public InformationBuildResult(string message) : base(LogLevel.Debug, message, false)
    {
    }
}
