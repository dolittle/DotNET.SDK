// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation;
using FluentValidation.Internal;

namespace Dolittle.Validation
{
    /// <summary>
    /// Represents the rule for a model of any type.
    /// </summary>
    /// <typeparam name="T">Type the rule represents.</typeparam>
    public class ModelRule<T> : PropertyRule
    {
        /// <summary>
        /// Property name for a model rule.
        /// </summary>
        public const string ModelRulePropertyName = "ModelRuleProperty";

        static readonly PropertyInfo InternalProperty;
        static readonly Func<object, object> InternalFunc = (o) => o;
        static readonly Expression<Func<T, object>> InternalExpression = (T o) => o;

        static ModelRule()
        {
            InternalProperty = typeof(ModelRule<T>).GetTypeInfo().GetProperty(ModelRulePropertyName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelRule{T}"/> class.
        /// </summary>
        public ModelRule()
            : base(
                InternalProperty,
                InternalFunc,
                InternalExpression,
                () => CascadeMode.StopOnFirstFailure,
                InternalProperty.PropertyType,
                typeof(T))
        {
        }

        /// <summary>
        /// Gets or sets the model rule property.
        /// </summary>
        public static string ModelRuleProperty { get; set; }
    }
}
