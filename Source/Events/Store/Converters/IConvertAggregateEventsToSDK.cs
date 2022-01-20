// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using PbCommittedAggregateEvents = Dolittle.Runtime.Events.Contracts.CommittedAggregateEvents;

namespace Dolittle.SDK.Events.Store.Converters;

/// <summary>
/// Defines a system that is capable of converting Aggregate events to SDK.
/// </summary>
public interface IConvertAggregateEventsToSDK
{
    /// <summary>
    /// Convert from <see cref="PbCommittedAggregateEvents"/> to <see cref="CommittedAggregateEvents"/>.
    /// </summary>
    /// <param name="source"><see cref="PbCommittedAggregateEvents"/>.</param>
    /// <param name="events">When the method returns, the converted <see cref="CommittedAggregateEvents"/> if conversion was successful, otherwise null.</param>
    /// <param name="error">When the method returns, null if the conversion was successful, otherwise the error that caused the failure.</param>
    /// <returns>A value indicating whether or not the conversion was successful.</returns>
    bool TryConvert(PbCommittedAggregateEvents source, out CommittedAggregateEvents events, out Exception error);
}