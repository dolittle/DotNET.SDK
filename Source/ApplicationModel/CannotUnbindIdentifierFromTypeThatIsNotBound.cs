// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.ApplicationModel;

/// <summary>
/// Exception that gets thrown when attempting to unbind an identifier from a type that it is not bound to.
/// </summary>
public class CannotUnbindIdentifierFromTypeThatIsNotBound : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CannotUnbindIdentifierFromTypeThatIsNotBound"/> class.
    /// </summary>
    /// <param name="identifier">The <see cref="IIdentifier"/> that was attempted to unbind.</param>
    /// <param name="type">The type that was attempted to unbind from.</param>
    public CannotUnbindIdentifierFromTypeThatIsNotBound(IIdentifier identifier, Type type)
        : base($"Cannot unbind {identifier} from {type}. It was not bound")
    {
    }
}
