// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Projections.Store.for_ProjectionStore.given;

[Projection("DA61A6ED-586C-4DD4-AB97-161310453400")]
public class a_decorated_projection_type
{
    public int Value { get; set; }
    
}
public class an_undecorated_projection_type
{
    public int Value { get; set; }
}

[Projection("33390DB3-1223-4524-BACC-9DD27E7469F9")]
public class a_decorated_projection_type_with_lower_case_property
{
    public int value { get; set; }
}
[Projection("22B62AD9-DF91-4630-A3C4-FD420276BFC9", "264C8022-9F88-4320-B483-AF63752AB420")]
public class a_decorated_projection_type_with_scope
{
    public int Value { get; set; }
}