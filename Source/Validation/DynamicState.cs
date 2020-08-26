// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace Dolittle.Validation
{
    /// <summary>
    /// Represents state used in validation.
    /// </summary>
    public class DynamicState : DynamicObject
    {
        readonly object _model;
        readonly List<PropertyInfo> _properties = new List<PropertyInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicState"/> class.
        /// </summary>
        /// <param name="model">Model to use as base for representing the state.</param>
        public DynamicState(object model)
        {
            _model = model;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicState"/> class.
        /// </summary>
        /// <param name="model">Model to use as base for representing the state.</param>
        /// <param name="properties">Properties that are supported.</param>
        public DynamicState(object model, IEnumerable<PropertyInfo> properties)
        {
            _model = model;
            _properties.AddRange(properties);
        }

        /// <inheritdoc/>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            foreach (var property in _properties)
            {
                if (property.Name.Equals(binder.Name, StringComparison.InvariantCulture))
                {
                    var actualModel = GetActualModel(_model, property.DeclaringType);
                    result = property.GetValue(actualModel, null);
                    return true;
                }
            }

            result = null;
            return false;
        }

        object GetActualModel(object model, Type targetType)
        {
            var modelType = model.GetType();
            if (modelType == targetType)
                return model;

            foreach (var property in modelType.GetTypeInfo().GetProperties())
            {
                var propertyValue = property.GetValue(model, null);
                if (property.PropertyType == targetType)
                {
                    return propertyValue;
                }
                else
                {
                    var result = GetActualModel(propertyValue, targetType);
                    if (result != null)
                        return result;
                }
            }

            return null;
        }
    }
}
