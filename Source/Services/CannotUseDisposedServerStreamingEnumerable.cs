// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Services;

/// <summary>
/// Exception that gets thrown when using a <see cref="ServerStreamingEnumerable{TServerMessage}"/> that is disposed.
/// </summary>
public class CannotUseDisposedServerStreamingEnumerable : Exception
{
    /// <summary>
    /// Initializes an instance of the <see cref="CannotUseDisposedServerStreamingEnumerable"/> class.
    /// </summary>
    public CannotUseDisposedServerStreamingEnumerable()
        : base($"Cannot use {typeof(ServerStreamingEnumerable<>).Name} when already disposed off")
    {
    }
}
