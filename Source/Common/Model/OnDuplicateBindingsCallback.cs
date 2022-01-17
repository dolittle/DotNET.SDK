// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Common.Model;

/// <summary>
/// The action to perform on duplicate <see cref="IdentifierMapBinding{TValue}"/> bindings.
/// </summary>
/// <typeparam name="TValue">The <see cref="Type"/> of the value bound to the <see cref="IIdentifier"/></typeparam>
public delegate void OnDuplicateBindingsCallback<in TValue>(IBinding binding, TValue value, int numDuplicates);
