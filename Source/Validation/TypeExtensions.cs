// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Reflection;

namespace Dolittle.Validation
{
    /// <summary>
    /// Contains Type extension specific to validation.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Check if a type is a ModelRuleProperty{T}.
        /// </summary>
        /// <param name="type"><see cref="Type"/> being extended.</param>
        /// <returns>True if this type is a ModelRuleProperty, otherwise false.</returns>
        public static bool IsModelRuleProperty(this Type type)
        {
            return type.IsDerivedFromOpenGeneric(typeof(ModelRule<>));
        }
    }
}