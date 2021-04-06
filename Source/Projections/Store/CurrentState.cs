// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Projections.Store
{
    /// <summary>
    /// Represents the current projection state.
    /// </summary>
    /// <typeparam name="TProjection">The <see cref="System.Type" /> of the projection.</typeparam>
    public class CurrentState<TProjection>
        where TProjection : class, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentState{TProjection}"/> class.
        /// </summary>
        /// <param name="state">The current <typeparamref name="TProjection"/> state.</param>
        /// <param name="type">The <see cref="CurrentStateType" />.</param>
        public CurrentState(TProjection state, CurrentStateType type)
        {
            State = state;
            Type = type;
        }

        /// <summary>
        /// Gets the <see cref="CurrentStateType" />.
        /// </summary>
        public CurrentStateType Type { get; }

        /// <summary>
        /// Gets the state.
        /// </summary>
        public TProjection State { get; }
    }
}
