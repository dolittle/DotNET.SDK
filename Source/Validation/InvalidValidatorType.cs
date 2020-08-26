// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Validation
{
    /// <summary>
    /// Exception that gets thrown when a validator type is of wrong type.
    /// </summary>
    public class InvalidValidatorType : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidValidatorType"/> class.
        /// </summary>
        public InvalidValidatorType()
            : base($"Dynamic state is only supported on a property validator that inherits from {typeof(PropertyValidatorWithDynamicState)}")
        {
        }
    }
}
