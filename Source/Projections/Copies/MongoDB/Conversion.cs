// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Represents MongoDB read model field conversion
/// </summary>
public enum Conversion : ushort
{
    /// <summary>
    /// Do not convert the field.
    /// </summary>
    None = 0,

    /// <summary>
    /// Converts the field into a BSON Date represented as array.
    /// </summary>
    DateAsArray,
    
    /// <summary>
    /// Converts the field into a BSON Date represented as date.
    /// </summary>
    DateAsDate,
    
    /// <summary>
    /// Converts the field into a BSON Date represented as document.
    /// </summary>
    DateAsDocument,
    
    /// <summary>
    /// Converts the field into a BSON Date represented as int64.
    /// </summary>
    DateAsInt64,
    
    /// <summary>
    /// Converts the field into a BSON Date represented as string.
    /// </summary>
    DateAsString,

    /// <summary>
    /// Converts the field into a GUID represented as string.
    /// </summary>
    GuidAsString,
    
    /// <summary>
    /// Converts the field into a GUID represented as standard binary.
    /// </summary>
    GuidAsStandardBinary,
    
    /// <summary>
    /// Converts the field into a GUID represented as standard binary.
    /// </summary>
    GuidAsCSharpLegacyBinary,
}
