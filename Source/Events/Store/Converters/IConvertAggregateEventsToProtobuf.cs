// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using PbUncommittedAggregateEvents = Dolittle.Runtime.Events.Contracts.UncommittedAggregateEvents;

namespace Dolittle.SDK.Events.Store.Converters;

/// <summary>
/// Defines a system that is capable of converting aggregate events to protobuf.
/// </summary>
public interface IConvertAggregateEventsToProtobuf
{
    /// <summary>
    /// Convert from <see cref="UncommittedAggregateEvents"/> to <see cref="PbUncommittedAggregateEvents"/>.
    /// </summary>
    /// <param name="source"><see cref="UncommittedAggregateEvents"/>.</param>
    /// <param name="events">When the method returns, the converted <see cref="PbUncommittedAggregateEvents"/> if conversion was successful, otherwise null.</param>
    /// <param name="error">When the method returns, null if the conversion was successful, otherwise the error that caused the failure.</param>
    /// <returns>A value indicating whether or not the conversion was successful.</returns>
    bool TryConvert(UncommittedAggregateEvents? source, out PbUncommittedAggregateEvents? events, out Exception? error);
}
