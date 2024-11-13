// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Dolittle.SDK.Common;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Represents the MongoDB copy collection name for a projection.
/// </summary>
/// <param name="Value">The collection name.</param>
public partial record MongoDBProjectionCollectionName(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Implicitly converts the <see cref="string"/> to <see cref="MongoDBProjectionCollectionName"/>.
    /// </summary>
    /// <param name="value">The <see cref="string"/>.</param>
    /// <returns>The <see cref="MongoDBProjectionCollectionName"/>.</returns>
    public static implicit operator MongoDBProjectionCollectionName(string value) => new(value);

    /// <summary>
    /// Implicitly converts the <see cref="MongoDBProjectionCollectionName"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="name">The <see cref="MongoDBProjectionCollectionName"/>.</param>
    /// <returns>The <see cref="string"/>.</returns>
    public static implicit operator string(MongoDBProjectionCollectionName name) => name.Value;

    /// <summary>
    /// Try get the <see cref="MongoDBProjectionCollectionName"/> from a <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="TSchema">The <see cref="Type"/> of the projection.</typeparam>
    /// <returns>The <see cref="MongoDBProjectionCollectionName"/> derived from the <see cref="CopyProjectionToMongoDBAttribute"/> or the <see cref="MemberInfo.Name"/>.</returns>
    public static MongoDBProjectionCollectionName GetFrom<TSchema>()
        where TSchema : class
        => GetFrom(typeof(TSchema));

    /// <summary>
    /// Try get the <see cref="MongoDBProjectionCollectionName"/> from a <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to get the <see cref="MongoDBProjectionCollectionName"/> from.</param>
    /// <returns>The <see cref="MongoDBProjectionCollectionName"/> derived from the <see cref="CopyProjectionToMongoDBAttribute"/> or the <see cref="MemberInfo.Name"/>.</returns>
    static MongoDBProjectionCollectionName GetFrom(Type type)
    {
        if (!type.TryGetDecorator<ProjectionAttribute>(out var projectionAttribute))
            throw new ArgumentException("Type must be a projection");

        var projectionModelId = projectionAttribute.GetIdentifier(type);

        return From(projectionModelId.Id, projectionModelId.Alias);
    }

    public static MongoDBProjectionCollectionName From(ProjectionId projectionId, string? alias)
    {
        var name = Trimmed(alias ?? "", 25);
        // Replace anything but letters and numbers with underscore
        name = _aliasReplacerRegex.Replace(name, "_");
        return new MongoDBProjectionCollectionName($"proj_{name}_{projectionId.Value:N}");
    }

    static string Trimmed(string input, int maxLength)
    {
        if (input.Length > maxLength)
            return input[..maxLength];

        return input;
    }

    static readonly System.Text.RegularExpressions.Regex _aliasReplacerRegex = AliasReplaceRegex();

    [System.Text.RegularExpressions.GeneratedRegex("[^a-zA-Z0-9]")]
    private static partial System.Text.RegularExpressions.Regex AliasReplaceRegex();
}
