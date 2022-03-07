// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Projections;
using Microsoft.Extensions.Logging;

namespace Dolittle.SDK.Embeddings.Store;

/// <summary>
/// Log messages for <see cref="Dolittle.SDK.Embeddings.Store"/>.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "Getting current embedding state with key {Key} for embedding of {EmbeddingType} with id {EmbeddingId}")]
    internal static partial void Get(this ILogger logger, Key key, Type embeddingType, EmbeddingId embeddingId);

    [LoggerMessage(0, LogLevel.Error, "The embedding state returned by the Runtime could not be converted to {EmbeddingType}.")]
    internal static partial void FailedToConvertState(this ILogger logger, Type embeddingType, Exception exception);
    
    [LoggerMessage(0, LogLevel.Debug, "Getting all current embedding states for embedding of {EmbeddingType} with id {EmbeddingId}")]
    internal static partial void GetAll(this ILogger logger, Type embeddingType, EmbeddingId embeddingId);
    
    [LoggerMessage(0, LogLevel.Debug, "Getting all keys for embedding of {EmbeddingType} with id {EmbeddingId}")]
    internal static partial void GetKeys(this ILogger logger, Type embeddingType, EmbeddingId embeddingId);
}
