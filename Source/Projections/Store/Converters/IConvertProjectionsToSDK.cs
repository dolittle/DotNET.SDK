// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using PbCurrentState = Dolittle.Runtime.Events.Processing.Contracts.ProjectionCurrentState;

namespace Dolittle.SDK.Projections.Store.Converters
{
    /// <summary>
    /// Defines a system that is capable of converting projections to SDK.
    /// </summary>
    public interface IConvertProjectionsToSDK
    {
        /// <summary>
        /// Convert from <see cref="PbCurrentState"/> to <see cref="CurrentState{TProjection}"/>.
        /// </summary>
        /// <param name="source"><see cref="PbCurrentState"/>.</param>
        /// <param name="state">When the method returns, the converted <see cref="CurrentState{TProjection}"/> if conversion was successful, otherwise null.</param>
        /// <param name="error">When the method returns, null if the conversion was successful, otherwise the error that caused the failure.</param>
        /// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
        /// <returns>A value indicating whether or not the conversion was successful.</returns>
        bool TryConvert<TProjection>(PbCurrentState source, out CurrentState<TProjection> state, out Exception error)
            where TProjection : class, new();

        /// <summary>
        /// Convert from <see cref="IEnumerable{T}"/> of type <see cref="PbCurrentState"/> to <see cref="IEnumerable{T}" /> of <see cref="CurrentState{TProjection}"/>.
        /// </summary>
        /// <param name="source"><see cref="IEnumerable{T}"/> of type <see cref="PbCurrentState"/>.</param>
        /// <param name="states">When the method returns, the converted <see cref="IEnumerable{T}" /> of <see cref="CurrentState{TProjection}"/> if conversion was successful, otherwise null.</param>
        /// <param name="error">When the method returns, null if the conversion was successful, otherwise the error that caused the failure.</param>
        /// <typeparam name="TProjection">The <see cref="Type" /> of the projection.</typeparam>
        /// <returns>A value indicating whether or not the conversion was successful.</returns>
        bool TryConvert<TProjection>(IEnumerable<PbCurrentState> source, out IEnumerable<CurrentState<TProjection>> states, out Exception error)
            where TProjection : class, new();
    }
}
