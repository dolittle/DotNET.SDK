// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Protobuf.Contracts;
using Dolittle.SDK.Concepts;
using Google.Protobuf;

namespace Dolittle.SDK.Protobuf;

/// <summary>
/// Conversion extensions for converting between <see cref="Guid"/> and <see cref="Uuid"/>.
/// </summary>
public static class GuidExtensions
{
    /// <summary>
    /// Convert a <see cref="Guid"/> to a <see cref="Uuid"/>.
    /// </summary>
    /// <param name="id">The <see cref="Guid"/> to convert.</param>
    /// <returns>The converted <see cref="Uuid"/>.</returns>
    public static Uuid ToProtobuf(this Guid id) => new() { Value = ByteString.CopyFrom(id.ToByteArray()) };

    /// <summary>
    /// Convert a <see cref="ConceptAs{Guid}"/> to a <see cref="Uuid"/>.
    /// </summary>
    /// <param name="id">The <see cref="ConceptAs{Guid}"/> to convert.</param>
    /// <returns>The converted <see cref="Uuid"/>.</returns>
    public static Uuid ToProtobuf(this ConceptAs<Guid> id) => id.Value.ToProtobuf();

    /// <summary>
    /// Convert a <see cref="Uuid"/> to a <see cref="Guid"/>.
    /// </summary>
    /// <param name="source">The <see cref="Uuid"/> to convert.</param>
    /// <param name="id">When the method returns, the converted <see cref="Guid"/> if conversion was successful, otherwise the default value.</param>
    /// <param name="error">When the method returns, null if the conversion was successful, otherwise the error that caused the failure.</param>
    /// <returns>A value indicating whether or not the conversion was successful.</returns>
    public static bool TryToGuid(this Uuid source, out Guid id, out Exception error)
    {
        id = default;
        if (source == null || source.Value == null)
        {
            error = new InvalidGuidConversion("value was null");
            return false;
        }

        if (source.Value.Length != 16)
        {
            error = new InvalidGuidConversion("the value did not contain 16 bytes");
            return false;
        }

        try
        {
            id = new Guid(source.Value.ToByteArray());
            error = null;
            return true;
        }
        catch (Exception ex)
        {
            error = new InvalidGuidConversion(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Convert a <see cref="Uuid"/> to a <see cref="Guid"/>.
    /// </summary>
    /// <param name="source">The <see cref="Uuid"/> to convert.</param>
    /// <returns>The converted <see cref="Guid"/>.</returns>
    public static Guid ToGuid(this Uuid source)
        => TryToGuid(source, out var id, out var error) ? id : throw error;

    /// <summary>
    /// Convert a <see cref="Uuid"/> to a <see cref="ConceptAs{T}"/>.
    /// </summary>
    /// <param name="source">The <see cref="Uuid"/> to convert.</param>
    /// <param name="id">When the method returns, the converted <see cref="ConceptAs{T}"/> if conversion was successful, otherwise the default value.</param>
    /// <param name="error">When the method returns, null if the conversion was successful, otherwise the error that caused the failure.</param>
    /// <typeparam name="T">Type to convert to.</typeparam>
    /// <returns>A value indicating whether or not the conversion was successful.</returns>
    public static bool TryTo<T>(this Uuid source, out T id, out Exception error)
        where T : ConceptAs<Guid>
    {
        if (TryToGuid(source, out var value, out error))
        {
            id = typeof(T).GetConstructor(new []{ typeof(Guid) })?.Invoke(new object[]{ value }) as T;
            return true;
        }

        id = null;
        return false;
    }

    /// <summary>
    /// Convert a <see cref="Uuid"/> to a <see cref="ConceptAs{T}"/>.
    /// </summary>
    /// <param name="source">The <see cref="Uuid"/> to convert.</param>
    /// <typeparam name="T">Type to convert to.</typeparam>
    /// <returns>The converted <see cref="ConceptAs{T}"/>.</returns>
    public static T To<T>(this Uuid source)
        where T : ConceptAs<Guid>
        => TryTo<T>(source, out var id, out var error) ? id : throw error;
}
