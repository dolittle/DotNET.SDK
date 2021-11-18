// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Resources.MongoDB;

namespace Dolittle.SDK.Resources
{
    /// <summary>
    /// Defines a system that knows all the resources for a Tenant.
    /// </summary>
    public interface IResources
    {
        /// <summary>
        /// Gets the <see cref="IMongoDBResource"/>.
        /// </summary>
        IMongoDBResource MongoDB { get; }
    }
}
