// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Resources.MongoDB;

namespace Dolittle.SDK.Resources
{
    /// <summary>
    /// Represents an implementation of <see cref="IResources"/>.
    /// </summary>
    public class Resources : IResources
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Resources"/> class.
        /// </summary>
        /// <param name="mongoDBResource">The <see cref="IMongoDBResource"/>.</param>
        public Resources(IMongoDBResource mongoDBResource)
        {
            MongoDB = mongoDBResource;
        }

        /// <inheritdoc />
        public IMongoDBResource MongoDB { get; }
    }
}