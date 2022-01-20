// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Failures;

/// <summary>
/// Exception that gets thrown when an unknown <see cref="Failure"/> is returned from the Runtime.
/// </summary>
public class UnknownRuntimeFailure : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnknownRuntimeFailure"/> class.
    /// </summary>
    /// <param name="failure">The <see cref="Failure"/> that was returned from the Runtime.</param>
    public UnknownRuntimeFailure(Failure failure)
        : base($"The Runtime returned an unknown failure with id '{failure.Id}'. {failure.Reason}")
    {
    }
}
