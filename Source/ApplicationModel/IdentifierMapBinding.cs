// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.ApplicationModel;

/// <summary>
/// Represents the mapped binding in an <see cref="IdentifierMap{TValue}"/>.
/// </summary>
/// <param name="Binding">The <see cref="IBinding"/>.</param>
/// <param name="BindingValue">The <typeparamref name="TValue"/> value that the binding is bound to.</param>
/// <typeparam name="TValue">The <see cref="Type"/> of the value that the binding is bound to. </typeparam>
public record IdentifierMapBinding<TValue>(IBinding Binding, TValue BindingValue);
