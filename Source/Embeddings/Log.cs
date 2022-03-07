// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Projections;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Embeddings;

/// <summary>
/// Log messages for <see cref="Dolittle.SDK.Embeddings"/>.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "Deleting embedding with key {Key} for embedding with id {EmbeddingId}")]
    internal static partial void Delete(this ILogger logger, Key key, EmbeddingId embeddingId);
    
    [LoggerMessage(0, LogLevel.Debug, "Updating embedding state with key {Key} for embedding of {EmbeddingType} with id {EmbeddingId}")]
    internal static partial void Update(this ILogger logger, Key key, Type embeddingType, EmbeddingId embeddingId);
    
    [LoggerMessage(0, LogLevel.Error, "The Runtime returned the embedding state '{State}'. But it could not be converted to {EmbeddingType}.")]
    internal static partial void FailedToConvertState(this ILogger logger, string state, Type embeddingType, Exception exception);
}
