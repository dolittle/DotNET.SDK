// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Common.ClientSetup;
using Dolittle.SDK.Security;
using PbBuildResult = Dolittle.Runtime.Client.Contracts.BuildResult;

namespace Dolittle.SDK.Protobuf;

/// <summary>
/// Conversion extensions for converting between <see cref="ClientBuildResult"/> and <see cref="PbBuildResult"/>.
/// </summary>
public static class ClientBuildResultExtension
{
    /// <summary>
    /// Converts the <see cref="ClientBuildResult"/> to <see cref="PbBuildResult"/>.
    /// </summary>
    /// <param name="buildResult">The <see cref="ClientBuildResult"/> to convert.</param>
    /// <returns>The converted <see cref="PbBuildResult"/>.</returns>
    public static PbBuildResult ToProtobuf(this ClientBuildResult buildResult)
        => new()
        {
            Message = buildResult.Message,
            Type = buildResult switch
            {
                InformationBuildResult => PbBuildResult.Types.Type.Information,
                FailureBuildResult => PbBuildResult.Types.Type.Failure,
                ErrorBuildResult => PbBuildResult.Types.Type.Error,
                _ => PbBuildResult.Types.Type.Information
            }
        };
}
