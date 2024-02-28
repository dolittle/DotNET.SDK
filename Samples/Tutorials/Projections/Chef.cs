// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Sample code for the tutorial at https://dolittle.io/tutorials/getting-started/projections/

using System.Collections.Generic;
using Dolittle.SDK.Projections;

public class Chef: ProjectionBase
{
    public string Name = "";
    public List<string> Dishes = new();
}
