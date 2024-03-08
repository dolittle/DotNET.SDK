// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.SDK.Events;

namespace Dolittle.SDK.Projections.Types;

[EventType("b118f2ff-68a5-46bc-949b-ec849b48ce02")]
public class SomeOtherEvent
{
    public int SomeNumber { get; set; }
}
