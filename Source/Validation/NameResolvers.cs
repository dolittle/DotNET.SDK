// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Dolittle.Concepts;
using Dolittle.Reflection;
using FluentValidation.Internal;

namespace Dolittle.Validation
{
    /// <summary>
    /// Resolves property names and display names, taking into account concepts and model rules.
    /// </summary>
    public static class NameResolvers
    {
        /// <summary>
        /// Use by Fluent Validation.  Resolves property names, taking into account concepts and model rules.
        /// </summary>
        /// <param name="type"><see cref="Type"/> to resolve from.</param>
        /// <param name="memberInfo"><see cref="MemberInfo"/> for property.</param>
        /// <param name="expression"><see cref="LambdaExpression"/> representing the property.</param>
        /// <returns>Name of property.</returns>
        public static string PropertyNameResolver(Type type, MemberInfo memberInfo, LambdaExpression expression)
        {
            if (expression != null)
            {
                var chain = FromExpression(type, memberInfo, expression);
                if (chain.Count > 0)
                {
                    return chain.ToString();
                }
            }

            if (memberInfo != null)
            {
                return (IsModelRule(memberInfo) || IsConcept(memberInfo))
                    ? string.Empty : memberInfo.Name;
            }

            return null;
        }

        /// <summary>
        /// Used by Fluent Validation.  Resolves display name based on member expression.
        /// </summary>
        /// <param name="type"><see cref="Type"/> to resolve from.</param>
        /// <param name="memberInfo"><see cref="MemberInfo"/> for property.</param>
        /// <param name="expression"><see cref="LambdaExpression"/> representing the property.</param>
        /// <returns>Name of property.</returns>
#pragma warning disable CA1801
        public static string DisplayNameResolver(Type type, MemberInfo memberInfo, LambdaExpression expression)
#pragma warning restore CA1801
        {
            if (expression == null) return "[N/A]";
            return FromExpression(expression);
        }

        static PropertyChain FromExpression(Type type, MemberInfo memberInfo, LambdaExpression expression)
        {
            var stack = new Stack<string>();

            if (type.IsConcept())
                return new PropertyChain(stack);

            for (var memberExpression = ExpressionExtensions.Unwrap(expression.Body);
                    memberExpression != null;
                    memberExpression = ExpressionExtensions.Unwrap(memberExpression.Expression))
            {
                if (!IsConcept(memberInfo))
                    stack.Push(memberExpression.Member.Name);
            }

            return new PropertyChain((IEnumerable<string>)stack);
        }

        static string FromExpression(LambdaExpression expression)
        {
            var stack = new Stack<string>();
            for (var memberExpression = ExpressionExtensions.Unwrap(expression.Body);
                    memberExpression != null;
                    memberExpression = ExpressionExtensions.Unwrap(memberExpression.Expression))
            {
                stack.Push(memberExpression.Member.Name);
            }

            return string.Join(".", stack.ToArray());
        }

        static bool IsModelRule(MemberInfo memberInfo)
        {
            return memberInfo.DeclaringType.IsModelRuleProperty();
        }

        static bool IsConcept(MemberInfo memberInfo)
        {
            return memberInfo.DeclaringType.IsConcept();
        }
    }
}