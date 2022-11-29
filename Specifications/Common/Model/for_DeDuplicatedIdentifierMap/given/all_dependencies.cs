// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.SDK.Concepts;

namespace Dolittle.SDK.Common.Model.for_DeDuplicatedIdentifierMap.given;

public class all_dependencies
{
    
}
public record an_id(Guid Value) : ConceptAs<Guid>(Value);
public class an_identifier : Identifier<an_id>
{
    public an_identifier(an_id id) : base("an_id", id, "alias")
    {
    }
}

