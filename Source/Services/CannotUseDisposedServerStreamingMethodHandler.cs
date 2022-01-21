// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Services;

/// <summary>
/// Exception that gets thrown when using a <see cref="ServerStreamingMethodHandler{TServerMessage}"/> that is disposed.
/// </summary>
public class CannotUseDisposedServerStreamingMethodHandler : Exception
{
    /// <summary>
    /// Initializes an instance of the <see cref="CannotUseDisposedServerStreamingMethodHandler"/> class.
    /// </summary>
    public CannotUseDisposedServerStreamingMethodHandler()
        : base($"Cannot use {typeof(ServerStreamingMethodHandler<>).Name} when already disposed off")
    {
    }
}
