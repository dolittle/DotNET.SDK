// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Dolittle.SDK.Common;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Represents the MongoDB copy collection name for a projection.
/// </summary>
/// <param name="Value">The collection name.</param>
public partial record MongoDBCopyCollectionName(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Implicitly converts the <see cref="string"/> to <see cref="MongoDBCopyCollectionName"/>.
    /// </summary>
    /// <param name="value">The <see cref="string"/>.</param>
    /// <returns>The <see cref="MongoDBCopyCollectionName"/>.</returns>
    public static implicit operator MongoDBCopyCollectionName(string value) => new(value);

    /// <summary>
    /// Implicitly converts the <see cref="MongoDBCopyCollectionName"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="name">The <see cref="MongoDBCopyCollectionName"/>.</param>
    /// <returns>The <see cref="string"/>.</returns>
    public static implicit operator string(MongoDBCopyCollectionName name) => name?.Value;

    /// <summary>
    /// Try get the <see cref="MongoDBCopyCollectionName"/> from a <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="TSchema">The <see cref="Type"/> of the projection.</typeparam>
    /// <returns>The <see cref="MongoDBCopyCollectionName"/> derived from the <see cref="CopyProjectionToMongoDBAttribute"/> or the <see cref="MemberInfo.Name"/>.</returns>
    public static MongoDBCopyCollectionName GetFrom<TSchema>()
        where TSchema : class
        => GetFrom(typeof(TSchema));

    /// <summary>
    /// Try get the <see cref="MongoDBCopyCollectionName"/> from a <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to get the <see cref="MongoDBCopyCollectionName"/> from.</param>
    /// <returns>The <see cref="MongoDBCopyCollectionName"/> derived from the <see cref="CopyProjectionToMongoDBAttribute"/> or the <see cref="MemberInfo.Name"/>.</returns>
    static MongoDBCopyCollectionName GetFrom(Type type)
    {
        if (!type.TryGetDecorator<ProjectionAttribute>(out var projectionAttribute))
            throw new ArgumentException("Type must be a projection");

        var projectionModelId = projectionAttribute.GetIdentifier(type);

        return From(projectionModelId);
    }

    static MongoDBCopyCollectionName From(ProjectionModelId projectionModelId)
    {
        var name = Trimmed(projectionModelId.Alias ?? "", 25);
        // Replace anything but letters and numbers with underscore
        name = _aliasReplacerRegex.Replace(name, "_");
        return new MongoDBCopyCollectionName($"proj_{name}_{projectionModelId.Id.Value:N}");
    }

    static string Trimmed(string input, int maxLength)
    {
        if (input.Length > maxLength)
            return input[..maxLength];

        return input;
    }

    static readonly System.Text.RegularExpressions.Regex _aliasReplacerRegex = new("[^a-zA-Z0-9]");
}
