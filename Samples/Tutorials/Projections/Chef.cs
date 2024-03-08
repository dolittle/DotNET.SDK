// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/getting-started/projections/

using System.Collections.Generic;
using Dolittle.SDK.Projections;

[Projection("e518986f-7e72-45f8-b4ac-2e34b26082ac")]
public class Chef: ReadModel
{
    public string Name = "";
    public List<string> Dishes = new();
}
