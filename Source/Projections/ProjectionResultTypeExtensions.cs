// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Contracts;

namespace Dolittle.SDK.Projections
{
    /// <summary>
    /// Extensions for <see cref="ProjectionResultType" />.
    /// </summary>
    public static class ProjectionResultTypeExtensions
    {
        /// <summary>
        /// Converts <see cref="ProjectionResultType" /> to <see cref="ProjectionNextStateType" />.
        /// </summary>
        /// <param name="resultType">The <see cref="ProjectionResultType" />.</param>
        /// <returns>The converted <see cref="ProjectionNextStateType" />.</returns>
        public static ProjectionNextStateType ToProtobuf(this ProjectionResultType resultType)
        {
            return resultType switch
            {
                ProjectionResultType.Replace => ProjectionNextStateType.Replace,
                ProjectionResultType.Delete => ProjectionNextStateType.Delete,
                _ => throw new UnknownProjectionResultType(resultType)
            };
        }
    }
}
