// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Copies.for_PropertyPathResolver.given;

public class all_dependencies
{
    protected static PropertyPathResolver resolver;
    protected static PropertyPath result;
    
    Establish context = () =>
    {
        resolver = new PropertyPathResolver();
    };

    protected static PropertyPath resolve_from_expression<TProperty>(Expression<Func<a_type, TProperty>> expression)
        => resolver.FromExpression(expression);
}