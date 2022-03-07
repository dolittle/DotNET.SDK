// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Events.Store.Converters;

/// <summary>
/// Exception that gets thrown when serialization of the content of an uncommitted event fails.
/// </summary>
public class CouldNotSerializeEventContent : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CouldNotSerializeEventContent"/> class.
    /// </summary>
    /// <param name="content">The event content that was serialized.</param>
    /// <param name="innerException">The error that caused the serialization to fail.</param>
    public CouldNotSerializeEventContent(object content, Exception innerException)
        : base($"Could not serialize the event content '{content}'", innerException)
    {
    }
}