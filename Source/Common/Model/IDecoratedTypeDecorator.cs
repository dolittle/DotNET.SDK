// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Common.Model;

/// <summary>
/// Defines the decorator for a unique binding.
/// </summary>
/// <typeparam name="TIdentifier">The <see cref="Type" /> of the unique identifier.</typeparam>
public interface IDecoratedTypeDecorator<out TIdentifier>
    where TIdentifier : IIdentifier
{
    /// <summary>
    /// Gets the <typeparamref name="TIdentifier"/>.
    /// </summary>
    /// <param name="decoratedType">The decorated <see cref="Type"/>.</param>
    /// <returns>The <typeparamref name="TIdentifier"/>.</returns>
    TIdentifier GetIdentifier(Type decoratedType);
}
