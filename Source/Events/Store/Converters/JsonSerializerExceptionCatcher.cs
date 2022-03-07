// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Dolittle.SDK.Events.Store.Converters;

/// <summary>
/// Represents an error handler for <see cref="JsonSerializerSettings"/> that captures exceptions that occur during serialization operation.
/// </summary>
public class JsonSerializerExceptionCatcher
{
    /// <summary>
    /// Gets a value indicating whether the serializer operation failed or not.
    /// </summary>
    public bool Failed { get; private set; }

    /// <summary>
    /// Gets the <see cref="Exception"/> that caused the serializer operation to fail.
    /// </summary>
    public Exception Error { get; private set; }

    /// <summary>
    /// Error handler for <see cref="JsonSerializerSettings"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="eventArgs">The event data.</param>
    public void OnError(object sender, ErrorEventArgs eventArgs)
    {
        Failed = true;
        Error = eventArgs.ErrorContext.Error;
        eventArgs.ErrorContext.Handled = true;
    }
}
