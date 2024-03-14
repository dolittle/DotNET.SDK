// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.SDK.Projections;

static class KeySelectorInstance<TKeySelector, TEvent> where TKeySelector : IKeySelector<TEvent>, new() where TEvent : class
{
    public static TKeySelector Mapper { get; } = new();
    public static KeySelector Instance { get; } = KeySelector.ByFunction<TEvent>(Mapper.Selector);
}

/// <summary>
/// Decorates a projection method with the <see cref="KeySelectorType.Function" />.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class KeyFromFunctionAttribute<TKeySelector, TEvent> : Attribute, IKeySelectorAttribute
    where TKeySelector : IKeySelector<TEvent>, new() where TEvent : class
{
    /// <inheritdoc/>
    public KeySelector KeySelector => KeySelectorInstance<TKeySelector, TEvent>.Instance;
}
