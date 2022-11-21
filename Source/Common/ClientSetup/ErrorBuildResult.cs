// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Common.ClientSetup;

/// <summary>
/// Represents the error <see cref="ClientBuildResult"/>.
/// </summary>
public class ErrorBuildResult : ClientBuildResult
{
    /// <summary>
    /// Initializes an instance of the <see cref="ErrorBuildResult"/> class.
    /// </summary>
    /// <param name="error">The error <see cref="Exception"/>.</param>
    public ErrorBuildResult(Exception error)
        : base(LogLevel.Error, error.Message, true, error)
    {
    }
}
