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
        // if (propertyExpression.Body is not MemberExpression member)
        // {
        //     throw new ArgumentException($"Expression {propertyExpression.Name} refers to a method, not a class member");
        // }
        //
        // propertyNames.Add(member.Member.Name);
        // while (member?.Expression is MemberExpression subExpression)
        // {
        //     member = subExpression;
        //     propertyNames.Add(member.Member.Name);
        // }
        var expressionToCheck = propertyExpression.Body;
        while (expressionToCheck is not null)
        {
            switch (expressionToCheck)
            {
                case MemberExpression memberExpression:
                    propertyNames.Add(memberExpression.Member.Name);
                    expressionToCheck = memberExpression.Expression;
                    break;
                default:
                    throw new ArgumentException($"Expression {propertyExpression.Name} must refer to a class member");
            }
        }

        return string.Join('.', propertyNames);
    }
}
