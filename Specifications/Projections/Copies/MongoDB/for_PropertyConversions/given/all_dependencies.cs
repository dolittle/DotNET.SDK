// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Machine.Specifications;

namespace Dolittle.SDK.Projections.Copies.MongoDB.for_PropertyConversions.given;

public class all_dependencies
{
    protected static PropertyConversions conversions;
    protected static IEnumerable<PropertyConversion> result;
    
    Establish context = () =>
    {
        conversions = new PropertyConversions();
    };

}