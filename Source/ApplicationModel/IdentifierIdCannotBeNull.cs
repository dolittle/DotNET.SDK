// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.ApplicationModel;

/// <summary>
/// Exception that gets thrown when the id of an identifier is null.
/// </summary>
public class IdentifierIdCannotBeNull : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IdentifierIdCannotBeNull"/> class.
    /// </summary>
    /// <param name="identifierType">The <see cref="Type"/> of the <see cref="Identifier{TId}"/>.</param>
    /// <param name="identifierIdType">The <see cref="Type"/> of the identifier id.</param>
    public IdentifierIdCannotBeNull(Type identifierType, Type identifierIdType)
        : base($"The {identifierIdType.Name} of an {identifierType.Name} identifier cannot be null")
    {
    }
}
