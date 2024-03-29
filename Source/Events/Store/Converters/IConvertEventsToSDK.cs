// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using PbCommittedEvent = Dolittle.Runtime.Events.Contracts.CommittedEvent;

namespace Dolittle.SDK.Events.Store.Converters;

/// <summary>
/// Defines a system that is capable of converting committed events to SDK.
/// </summary>
public interface IConvertEventsToSDK
{
    /// <summary>
    /// Convert from <see cref="PbCommittedEvent"/> to <see cref="CommittedEvent"/>.
    /// </summary>
    /// <param name="source"><see cref="PbCommittedEvent"/>.</param>
    /// <param name="event">When the method returns, the converted <see cref="CommittedEvent"/> if conversion was successful, otherwise null.</param>
    /// <param name="error">When the method returns, null if the conversion was successful, otherwise the error that caused the failure.</param>
    /// <returns>A value indicating whether or not the conversion was successful.</returns>
    bool TryConvert(PbCommittedEvent? source, [NotNullWhen(true)] out CommittedEvent? @event, [NotNullWhen(false)] out Exception? error);

    /// <summary>
    /// Convert from <see cref="IEnumerable{T}"/> of type <see cref="PbCommittedEvent"/> to <see cref="CommittedEvents"/>.
    /// </summary>
    /// <param name="source"><see cref="IEnumerable{T}"/> of type <see cref="PbCommittedEvent"/>.</param>
    /// <param name="events">When the method returns, the converted <see cref="CommittedEvents"/> if conversion was successful, otherwise null.</param>
    /// <param name="error">When the method returns, null if the conversion was successful, otherwise the error that caused the failure.</param>
    /// <returns>A value indicating whether or not the conversion was successful.</returns>
    bool TryConvert(IReadOnlyList<PbCommittedEvent> source, out CommittedEvents? events, out Exception? error);
}
