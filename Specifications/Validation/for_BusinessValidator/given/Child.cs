// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Validation.for_BusinessValidator.given
{
    public class Child
    {
        public ConceptAsLong ChildConcept { get; set; }

        public string ChildSimpleStringProperty { get; set; }

        public int ChildSimpleIntegerProperty { get; set; }

        public Grandchild Grandchild { get; set; }
    }
}