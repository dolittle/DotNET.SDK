// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Dolittle.Reflection;
using FluentValidation.Results;
using FluentValidation.Validators;

namespace Dolittle.Validation
{
    /// <summary>
    /// Represents a <see cref="PropertyValidator"/> that can hold dynamic state.
    /// </summary>
    public abstract class PropertyValidatorWithDynamicState : PropertyValidator
    {
        readonly List<PropertyInfo> _properties = new List<PropertyInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValidatorWithDynamicState"/> class.
        /// </summary>
        /// <param name="errorMessage">Error nessage.</param>
        protected PropertyValidatorWithDynamicState(string errorMessage)
            : base(errorMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValidatorWithDynamicState"/> class.
        /// </summary>
        /// <param name="errorMessageResourceName">Error nessage resource name.</param>
        /// <param name="errorMessageResourceType">Resource type that holds the resource.</param>
        protected PropertyValidatorWithDynamicState(string errorMessageResourceName, Type errorMessageResourceType)
            : base(errorMessageResourceName, errorMessageResourceType)
        {
        }

        /// <summary>
        /// Gets the properties representing the dynamic state.
        /// </summary>
        public IEnumerable<PropertyInfo> Properties => _properties;

        /// <summary>
        /// Gets the dynamic state.
        /// </summary>
        protected dynamic DynamicState { get; private set; }

        /// <inheritdoc/>
        public override IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context)
        {
            DynamicState = new DynamicState(context.Instance, Properties);
            return base.Validate(context);
        }

        /// <summary>
        /// Add an expression that resolve to a property.
        /// </summary>
        /// <typeparam name="T">Type that holds the property.</typeparam>
        /// <param name="expression">Expression to add.</param>
        public virtual void AddExpression<T>(Expression<Func<T, object>> expression)
        {
            var property = expression.GetPropertyInfo();
            _properties.Add(property);
        }
    }
}
