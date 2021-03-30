// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;

namespace Dolittle.SDK.Projections.Builder
{
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
        public KeySelector KeyFromProperty<TProperty>(Expression<Func<TEvent, TProperty>> propertyExpression)
        {
            if (propertyExpression.Body is MemberExpression memberExpression)
            {
                return new KeySelector(KeySelectorType.Property, memberExpression.Member.Name);
            }

            throw new KeySelectorExpressionWasNotAMemberExpression();
        }
    }
}
