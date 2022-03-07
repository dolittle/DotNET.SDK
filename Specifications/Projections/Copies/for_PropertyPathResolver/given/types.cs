// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Projections.Copies.for_PropertyPathResolver.given;

public class a_type
{
    public int Field;
    public int Property { get; set; }
    public a_type RecursiveField;
    public a_type RecursiveProperty { get; set; }

    public int Method() => 2;
    
}