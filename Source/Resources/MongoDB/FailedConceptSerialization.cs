// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Resources.MongoDB;

/// <summary>
/// Exception that gets thrown when the <see cref="ConceptSerializer{T}"/> failed serializing a concept.
/// </summary>
public class FailedConceptSerialization : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FailedConceptSerialization"/> class.
    /// </summary>
    /// <param name="msg">Message to display.</param>
    public FailedConceptSerialization(string msg)
        : base(msg)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FailedConceptSerialization"/> class.
    /// </summary>
    /// <param name="msg">Message to display.</param>
    /// <param name="error">The error.</param>
    public FailedConceptSerialization(string msg, Exception error)
        : base(msg, error)
    {
    }
}
