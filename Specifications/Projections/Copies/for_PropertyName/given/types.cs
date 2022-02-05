// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.SDK.Projections.Copies.for_PropertyName.given;


public class root_type
{
    public int Property { get; set; }
    public int PropertyWithoutSetter { get; }
    public int PropertyWithArrow => 2;
    public static int StaticProperty => 2;
    public int Field;
    public int InitializedField = 2;
    public readonly int ReadOnlyField = 2;
    public static int StaticField;
    public static int InitializedStaticField = 2;

    int PrivateProperty { get; set; }
    int PrivatePropertyWithArrow => 2;
    int PrivateField;
    readonly int ReadOnlyPrivateField = 2;
    public int[] IntArray;
    public complex_type ComplexField;
    public complex_type ComplexProperty { get; set; }

    public int Method() => 2;
    public static int StaticMethod() => 2;
}

public class complex_type
{
    public int NestedField;
    public int NestedProperty { get; set; }

    public inner_complex_type InnerComplexField;
    public inner_complex_type InnerComplexProperty { get; set; }
}

public class inner_complex_type
{
    public int NestedField;
    public int NestedProperty { get; set; }
}