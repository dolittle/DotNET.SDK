// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.SDK.Events.Store.Converters;
using Newtonsoft.Json;
using PbCurrentState = Dolittle.Runtime.Events.Processing.Contracts.ProjectionCurrentState;
using PbCurrentStateType = Dolittle.Runtime.Events.Processing.Contracts.ProjectionCurrentStateType;

namespace Dolittle.SDK.Projections.Store.Converters
{
    /// <summary>
    /// Represents an implementation of <see cref="IConvertProjectionsToSDK" />.
    /// </summary>
    public class ProjectionsToSDKConverter : IConvertProjectionsToSDK
    {
        /// <inheritdoc/>
        public bool TryConvert<TProjection>(PbCurrentState source, out CurrentState<TProjection> state, out Exception error)
            where TProjection : class, new()
            => TryDeserializeWithSettings(source, out state, out error);

        /// <inheritdoc/>
        public bool TryConvert<TProjection>(IEnumerable<PbCurrentState> source, out IEnumerable<CurrentState<TProjection>> states, out Exception error)
            where TProjection : class, new()
        {
            error = null;
            states = null;
            var currentStates = new List<CurrentState<TProjection>>();
            foreach (var protobufState in source)
            {
                if (!TryDeserializeWithSettings<TProjection>(protobufState, out var state, out error)) return false;
                currentStates.Add(state);
            }

            states = currentStates;
            return true;
        }

        bool TryDeserializeWithSettings<TProjection>(PbCurrentState currentState, out CurrentState<TProjection> projectionState, out Exception deserializationError)
            where TProjection : class, new()
        {
            projectionState = null;
            var exceptionCatcher = new JsonSerializerExceptionCatcher();
            var serializerSettings = new JsonSerializerSettings
            {
                Error = exceptionCatcher.OnError,
            };
            var stateType = GetCurrentStateType(currentState.Type);
            var state = JsonConvert.DeserializeObject<TProjection>(currentState.State, serializerSettings);

            if (exceptionCatcher.Failed)
                deserializationError = new CouldNotDeserializeProjection(currentState.State, currentState.Type, exceptionCatcher.Error);
            else
                deserializationError = null;

            projectionState = new CurrentState<TProjection>(state, stateType);

            return !exceptionCatcher.Failed;
        }

        CurrentStateType GetCurrentStateType(PbCurrentStateType type)
            => type switch
            {
                PbCurrentStateType.CreatedFromInitialState => CurrentStateType.CreatedFromInitialState,
                PbCurrentStateType.Persisted => CurrentStateType.Persisted,
                _ => throw new InvalidCurrentStateType(type)
            };
    }
}
