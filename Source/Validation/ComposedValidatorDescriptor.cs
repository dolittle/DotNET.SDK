// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Collections;
using FluentValidation;
using FluentValidation.Validators;

namespace Dolittle.Validation
{
    /// <summary>
    /// Combines multiples validator descriptors into a single validator descriptor.
    /// </summary>
    public class ComposedValidatorDescriptor : IValidatorDescriptor
    {
        readonly IList<IValidatorDescriptor> _registeredDescriptors;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComposedValidatorDescriptor"/> class.
        /// </summary>
        /// <param name="registeredDescriptors"><see cref="IEnumerable{T}"/> of <see cref="IValidatorDescriptor"/>.</param>
        public ComposedValidatorDescriptor(IEnumerable<IValidatorDescriptor> registeredDescriptors)
        {
            _registeredDescriptors = registeredDescriptors.ToList();
        }

        /// <summary>
        /// Gets the name display name for a property.
        /// </summary>
        /// <param name="property">Property to get name for.</param>
        /// <returns>DisplayName from descriptors.</returns>
        public string GetName(string property)
        {
            return _registeredDescriptors
                .Select(d => d.GetName(property))
                .FirstOrDefault(name => name != null);
        }

        /// <summary>
        /// Gets a collection of validators grouped by property.
        /// </summary>
        /// <returns><see cref="ILookup{TKey, TValue}"/>.</returns>
        public ILookup<string, IPropertyValidator> GetMembersWithValidators()
        {
            return _registeredDescriptors
                .Select(d => d.GetMembersWithValidators())
                .Combine();
        }

        /// <summary>
        /// Gets validators for a particular property.
        /// </summary>
        /// <param name="name">Member to get validators for.</param>
        /// <returns><see cref="IEnumerable{T}"/> of <see cref="IPropertyValidator"/>.</returns>
        public IEnumerable<IPropertyValidator> GetValidatorsForMember(string name)
        {
            return _registeredDescriptors
                .SelectMany(d => d.GetValidatorsForMember(name));
        }

        /// <summary>
        /// Gets rules for a property.
        /// </summary>
        /// <param name="name">Member to get validators for.</param>
        /// <returns><see cref="IEnumerable{T}"/> of <see cref="IValidationRule"/>.</returns>
        public IEnumerable<IValidationRule> GetRulesForMember(string name)
        {
            return _registeredDescriptors
                .SelectMany(d => d.GetRulesForMember(name));
        }
    }
}