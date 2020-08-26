// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentValidation;
using Machine.Specifications;

namespace Dolittle.Validation.for_BusinessValidator.given
{
    public class a_complex_object_graph_and_validator
    {
        protected static ParentValidator validator;
        protected static Parent parent;

        Establish context = () =>
            {
                ValidatorOptions.PropertyNameResolver = NameResolvers.PropertyNameResolver;
                ValidatorOptions.DisplayNameResolver = NameResolvers.DisplayNameResolver;

                parent = new Parent
                {
                    Id = -1,
                    SimpleIntegerProperty = 11,
                    SimpleStringProperty = "",
                    Child = new Child
                    {
                        ChildConcept = -2,
                        ChildSimpleIntegerProperty = 12,
                        ChildSimpleStringProperty = "",
                        Grandchild = new Grandchild
                        {
                            GrandchildConcept = -3,
                            GrandchildSimpleIntegerProperty = 13,
                            GrandchildSimpleStringProperty = ""
                        }
                    }
                };

                validator = new ParentValidator();
            };
    }
}