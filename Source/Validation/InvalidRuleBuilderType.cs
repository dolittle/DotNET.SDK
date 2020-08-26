// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Validation
{
    /// <summary>
    /// Exception that gets thrown when a rule builder is of the wrong type.
    /// </summary>
    public class InvalidRuleBuilderType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRuleBuilderType"/> class.
        /// </summary>
        /// <param name="type">Type of builder that is wrong.</param>
        public InvalidRuleBuilderType(Type type)
            : base($"Builder '{type.AssemblyQualifiedName}' is of wrong type - expecting RuleBuilder<>")
        {
        }
    }
}
