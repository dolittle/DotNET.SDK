// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;

namespace Dolittle.Validation
{
    public class ConceptAsString : ConceptAs<string>
    {
        public static implicit operator ConceptAsString(string value)
        {
            return new ConceptAsString { Value = value };
        }
    }
}