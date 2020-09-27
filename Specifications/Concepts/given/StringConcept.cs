// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Concepts.given
{
    public class StringConcept : ConceptAs<string>
    {
        public static implicit operator StringConcept(string value)
        {
            return new StringConcept { Value = value };
        }
    }
}