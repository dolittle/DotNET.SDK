// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Microservices;
using PbVersion = Dolittle.Versioning.Contracts.Version;

namespace Dolittle.SDK.Protobuf
{
    /// <summary>
    /// Conversion extensions for converting between <see cref="Version"/> and <see cref="PbVersion"/>.
    /// </summary>
    public static class VersionExtensions
    {
        /// <summary>
        /// Convert a <see cref="Version"/> to a <see cref="PbVersion"/>.
        /// </summary>
        /// <param name="version"><see cref="Version"/> to convert.</param>
        /// <returns>The converted <see cref="PbVersion"/>.</returns>
        public static PbVersion ToProtobuf(this Version version)
            => new PbVersion
                {
                    Major = version.Major,
                    Minor = version.Minor,
                    Patch = version.Patch,
                    Build = version.Build,
                    PreReleaseString = version.PreReleaseString
                };

        /// <summary>
        /// Convert a <see cref="PbVersion"/> to a <see cref="Version"/>.
        /// </summary>
        /// <param name="version"><see cref="PbVersion"/> to convert.</param>
        /// <returns>The converted <see cref="Version"/>.</returns>
        public static Version ToVersion(this PbVersion version)
            => new Version(
                version.Major,
                version.Minor,
                version.Patch,
                version.Build,
                version.PreReleaseString);
    }
}