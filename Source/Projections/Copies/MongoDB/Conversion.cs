// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Projections.Copies.MongoDB;

/// <summary>
/// Represents MongoDB read model field conversion
/// </summary>
public enum Conversion
{
    /// <summary>
    /// Converts the field into a BSON DateTime.
    /// </summary>
    DateTime = 1,

    /// <summary>
    /// Converts the field into a Guid familiar to BSON.
    /// </summary>
#pragma warning disable CA1720
    Guid = 2,
#pragma warning restore CA1720
}
