// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections;

/// <summary>
/// Represents the context of a projection.
/// </summary>
/// <param name="WasCreatedFromInitialState">A value indicating whether the projection was created from the initial state or retrieved from a persisted state.</param>
/// <param name="Key">The projection <see cref="Key"/>.</param>
/// <param name="EventContext">The <see cref="EventContext"/> in which the event occurred.</param>
public record ProjectionContext(bool WasCreatedFromInitialState, Key Key, EventContext EventContext);
