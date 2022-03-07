// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Contracts;

namespace Dolittle.SDK.Projections.Store.Converters;

/// <summary>
/// Exception that gets thrown when projection could not be deserialized.
/// </summary>
public class CouldNotDeserializeProjection : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CouldNotDeserializeProjection"/> class.
    /// </summary>
    /// <param name="state">The current projection state.</param>
    /// <param name="type">The <see cref="ProjectionCurrentStateType" />.</param>
    /// <param name="key">The <see cref="Key" />.</param>
    /// <param name="innerException">The inner deserialization <see cref="Exception" />.</param>
    public CouldNotDeserializeProjection(string state, ProjectionCurrentStateType type, Key key, Exception innerException)
        : base($"Could not deserialize projection with key {key}, type {type} and state '{state}'", innerException)
    {
    }
}