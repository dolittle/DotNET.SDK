// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dolittle.SDK.Projections.Copies;

/// <summary>
/// Represents an implementation of <see cref="IResolvePropertyPath"/>.
/// </summary>
public class PropertyPathResolver : IResolvePropertyPath
{
    /// <inheritdoc />
    public PropertyPath FromExpression<TSchema, TProperty>(Expression<Func<TSchema, TProperty>> propertyExpression) where TSchema : class
    {
        var propertyNames = new List<PropertyName>();
        var expressionToCheck = propertyExpression.Body;
        while (expressionToCheck is not null && expressionToCheck.NodeType != ExpressionType.Parameter)
        {
            switch (expressionToCheck.NodeType)
            {
                case ExpressionType.MemberAccess:
                    if (expressionToCheck is not MemberExpression memberExpression)
                    {
                        throw new ArgumentException($"Expression {propertyExpression.Name} must refer to a class member");
                    }
                    propertyNames.Add(memberExpression.Member.Name);
                    expressionToCheck = memberExpression.Expression;
                    break;
                default:
                    throw new ArgumentException($"Expression {propertyExpression.Name} must refer to a class member");
            }
        }
        // The member names gets added to the list in reverse order
        propertyNames.Reverse();
        return string.Join('.', propertyNames);
    }
}
