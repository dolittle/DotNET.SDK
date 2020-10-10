// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using PbUncommittedEvent = Dolittle.Runtime.Events.Contracts.UncommittedEvent;

namespace Dolittle.SDK.Events.Store.Converters
{
    /// <summary>
    /// Defines a system that is capable of converting events to protobuf.
    /// </summary>
    public interface IConvertEventsToProtobuf
    {
        /// <summary>
        /// Convert from <see cref="UncommittedEvents" /> to <see cref="IEnumerable{T}"/> of type <see cref="PbUncommittedEvent"/>.
        /// </summary>
        /// <param name="source"><see cref="UncommittedEvents"/>.</param>
        /// <param name="events">When the method returns, the converted <see cref="IEnumerable{T}"/> of type <see cref="PbUncommittedEvent"/>. if conversion was successful, otherwise null.</param>
        /// <param name="error">When the method returns, null if the conversion was successful, otherwise the error that caused the failure.</param>
        /// <returns>A value indicating whether or not the conversion was successful.</returns>
        bool TryConvert(UncommittedEvents source, out IEnumerable<PbUncommittedEvent> events, out Exception error);
    }
}
