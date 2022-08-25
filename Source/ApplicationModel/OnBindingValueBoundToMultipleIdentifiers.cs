// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.SDK.ApplicationModel;

/// <summary>
/// The action to perform on binding values bound to multiple <see cref="IdentifierMapBinding{TValue}"/> bindings.
/// </summary>
/// <typeparam name="TValue">The <see cref="Type"/> of the value bound to the <see cref="IIdentifier"/></typeparam>
public delegate void OnBindingValueBoundToMultipleIdentifiers<in TValue>(TValue value, IEnumerable<IIdentifier> identifiers);
