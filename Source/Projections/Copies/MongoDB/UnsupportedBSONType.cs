// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MongoDB.Bson;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Exception that gets thrown when an unsupported Bson Type is encountered.
/// </summary>
public class UnsupportedBSONType : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedBSONType"/> class.
    /// </summary>
    /// <param name="type"></param>
    public UnsupportedBSONType(BsonType type)
        : base($"The Bson Type {type} is not supported")
    {
    }
}
