// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Projections.Store
{
    /// <summary>
    /// Defines the different types a projection state can have.
    /// </summary>
    public enum CurrentStateType
    {
        /// <summary>
        /// The state was created from the initial state.
        /// </summary>
        CreatedFromInitialState = 0,

        /// <summary>
        /// The state was created from the persisted state.
        /// </summary>
        Persisted = 1
    }
}
