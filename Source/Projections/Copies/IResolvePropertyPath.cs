// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;

namespace Dolittle.SDK.Projections.Copies;

/// <summary>
/// Defines an system for resolving a <see cref="PropertyPath"/>.
/// </summary>
public interface IResolvePropertyPath
{
    /// <summary>
    /// Resolves a <see cref="PropertyPath"/> from an expression.
    /// </summary>
    /// <param name="propertyExpression">The expression for getting a property from the <typeparamref name="TSchema"/>.</param>
    /// <typeparam name="TSchema">The <see cref="Type"/> of the schema to resolve a <see cref="PropertyPath"/> for.</typeparam>
    /// <typeparam name="TProperty">The <see cref="Type"/> of the property.</typeparam>
    /// <returns>The <see cref="PropertyPath"/> resolved from the expression.</returns>
    PropertyPath FromExpression<TSchema, TProperty>(Expression<Func<TSchema, TProperty>> propertyExpression)
        where TSchema : class;
}
