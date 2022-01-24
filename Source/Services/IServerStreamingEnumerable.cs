// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Google.Protobuf;

namespace Dolittle.SDK.Services;

/// <summary>
/// Defines a system that can handle a gRPC server streaming method.
/// </summary>
/// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the server to the client.</typeparam>
public interface IServerStreamingEnumerable<out TServerMessage> : IDisposable, IAsyncEnumerable<TServerMessage>
{
}
