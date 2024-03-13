// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Builder;

/// <summary>
/// Represents a builder for building the key selector expression for projection's on() method.
/// </summary>
/// <typeparam name="TEvent">The <see cref="Type" /> of the event.</typeparam>
public class KeySelectorBuilder<TEvent> : KeySelectorBuilder
    where TEvent : class
{
    /// <summary>
    /// Select projection key from a property of the event.
    /// </summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="propertyExpression">The expression for getting the property on the event.</param>
    /// <returns>A <see cref="KeySelector"/>.</returns>
    /// <exception cref="KeySelectorExpressionWasNotAMemberExpression">Is thrown when the provided property expression is not a member expression.</exception>
    public KeySelector KeyFromProperty<TProperty>(Expression<Func<TEvent, TProperty>> propertyExpression)
    {
        if (propertyExpression.Body is MemberExpression memberExpression)
        {
            return KeySelector.Property(memberExpression.Member.Name);
        }

        throw new KeySelectorExpressionWasNotAMemberExpression();
    }

    /// <summary>
    /// Select projection key from a property of the event.
    /// </summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="function">The function for getting the projection key (id).</param>
    /// <returns>A <see cref="KeySelector"/>.</returns>
    /// <exception cref="KeySelectorExpressionWasNotAMemberExpression">Is thrown when the provided property expression is not a member expression.</exception>
    public KeySelector KeyFromFunction(Func<TEvent, EventContext, Key> function)
    {
        if (function is null) throw new ArgumentNullException(nameof(function));

        return KeySelector.ByFunction(function);
    }
}
