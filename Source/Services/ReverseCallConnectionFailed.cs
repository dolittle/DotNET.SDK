// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Protobuf.Contracts;

namespace Dolittle.SDK.Services;

/// <summary>
/// Exception that gets thrown when a failure occurs during the connect stage of a reverse call.
/// </summary>
public class ReverseCallConnectionFailed : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReverseCallConnectionFailed"/> class.
    /// </summary>
    /// <param name="failure">The <see cref="Failure" /> that occured.</param>
    public ReverseCallConnectionFailed(Failure failure)
        : base($"Failure occurred while connecting a reverse call. ${failure.Reason}")
    {
    }
}