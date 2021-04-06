// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Store
{
    /// <summary>
    /// Exception that gets thrown when there is no <see cref="Type" /> associated with a projection.
    /// </summary>
    public class NoTypeAssociatedWithProjection : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoTypeAssociatedWithProjection"/> class.
        /// </summary>
        /// <param name="projection">The <see cref="ProjectionId" />.</param>
        /// /// <param name="scope">The <see cref="ScopeId"/>.</param>
        public NoTypeAssociatedWithProjection(ProjectionId projection, ScopeId scope)
            : base($"There is not type associated with projection {projection} in scope {scope}")
        {
        }
    }
}
