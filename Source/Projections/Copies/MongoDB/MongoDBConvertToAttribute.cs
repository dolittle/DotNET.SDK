// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Decorates a projection read model property to indicate that the projection read model property should be converted to a specific BsonType.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class ConvertToMongoDBAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertToMongoDBAttribute"/> class.
    /// </summary>
    /// <param name="conversion">The <see cref="Copies.MongoDB.Conversion"/>.</param>
    public ConvertToMongoDBAttribute(Conversion conversion)
    {
        Conversion = conversion;
    }

    /// <summary>
    /// Gets the <see cref="Copies.MongoDB.Conversion"/> the decorated field.
    /// </summary>
    public Conversion Conversion { get; }
}
