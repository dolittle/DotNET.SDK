// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Protobuf.Contracts;
using Google.Protobuf;

namespace Dolittle.SDK.Services;

public interface IDisconnectProtocol<TClientMessage, TServerMessage>
    where TClientMessage : IMessage
    where TServerMessage : IMessage
{
    /// <summary>
    /// If supported by the protocol, creates a <typeparamref name="TClientMessage"/> that can be sent to the server to initiate a disconnect.
    /// </summary>
    /// <param name="gracePeriod"></param>
    /// <returns>A disconnect message, or null if not supported by the protocol</returns>
    TClientMessage? CreateInitiateDisconnectMessage(TimeSpan gracePeriod) => default;

    /// <summary>
    /// If supported by the protocol, lets the caller know if a message is a disconnect ack message.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    bool IsDisconnectAck(TServerMessage message) => false;

    /// <summary>
    /// If supported by the protocol, gets the failure if present from a disconnect ack message.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Failure? GetDisconnectFailure(TServerMessage message) => default;

    /// <summary>
    /// Lets the caller know if this protocol supports sending disconnect messages.
    /// </summary>
    bool SupportsDisconnectMessages => false;
}
