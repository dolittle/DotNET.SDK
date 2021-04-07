// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Projections
{
    /// <summary>
    /// Defines common structure for <see cref="KeySelector" /> attributes.
    /// </summary>
    public interface IKeySelectorAttribute
    {
        /// <summary>
        /// Gets the <see cref="Projections.KeySelector" />.
        /// </summary>
        KeySelector KeySelector { get; }
    }
}
