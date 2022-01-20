// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Common.Model;

/// <summary>
/// Exception that gets thrown when attempting to unbind an identifier from a processor builder that it is not bound to.
/// </summary>
public class CannotUnbindIdentifierFromProcessorBuilderThatIsNotBound : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CannotUnbindIdentifierFromProcessorBuilderThatIsNotBound"/> class.
    /// </summary>
    /// <param name="identifier">The <see cref="IIdentifier"/> that was attempted to unbind.</param>
    /// <param name="processorBuilder">The processor builder that was attempted to unbind from.</param>
    public CannotUnbindIdentifierFromProcessorBuilderThatIsNotBound(IIdentifier identifier, object processorBuilder)
        : base($"Cannot unbind {identifier} from {processorBuilder}. It was bot bound.")
    {
    }
}
